using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using VibraScan.Domain.Common;

namespace VibraScan.Infrastructure.Data.Bulk
{
    internal class SqlServerBulkOperations : IBulkOperations
    {
        private readonly ApplicationDbContext _context;

        private const int DefaultBatchSize = 2000;
        private const int DefaultTimeout = 300;

        private static readonly ConcurrentDictionary<Type, List<string>?> s_indexes = [];
        private static readonly ConcurrentDictionary<Type, object> s_managers = [];
        private static readonly ConcurrentDictionary<Type, List<ColumnDefinition>> s_columnDefinitions = [];

        public SqlServerBulkOperations(ApplicationDbContext context)
        {
            if (!context.Database.IsSqlServer())
            {
                throw new ArgumentException($"Неподдерживаемый провайдер: {context.Database.ProviderName}. Требуется SQL Server");
            }

            _context = context;
        }

        public async Task EnsureRangeAsync<T>(IEnumerable<T> entities, CancellationToken ct = default) where T : BaseEntity
        {
            if (entities == null) return;
            var entityList = entities as List<T> ?? [.. entities];
            if (entityList.Count == 0) return;

            var type = typeof(T);

            var uniqueIdxProps = s_indexes.GetOrAdd(type, GetUniqueIndexProperties);

            if ((uniqueIdxProps == null) || (uniqueIdxProps.Count == 0))
            {
                await BulkUpsertAsync(entityList, null, ct);
                return;
            }

            var manager = (BatchDuplicateManager<T>)s_managers.GetOrAdd(type, _ => new BatchDuplicateManager<T>(uniqueIdxProps));
            var buckets = manager.GroupByUniqueIndex(entityList);

            await BulkUpsertAsync(buckets.Keys.ToList(), uniqueIdxProps, ct);

            BatchDuplicateManager<T>.ApplyGeneratedIds(buckets);
        }

        private List<string>? GetUniqueIndexProperties(Type type)
        {
            var entityType = _context.Model.FindEntityType(type);

            var pkNames = entityType?.FindPrimaryKey()?.Properties
                .Select(p => p.Name)?.ToHashSet();

            return entityType?.GetIndexes()
                .Where(i => i.IsUnique && (pkNames == null || !i.Properties.All(p => pkNames.Contains(p.Name))))
                .Select(i => i.Properties.Select(p => p.Name).ToList())
                .FirstOrDefault();
        }

        private async Task BulkUpsertAsync<T>(List<T> entities, List<string>? uniqueIdxProps, CancellationToken ct) where T : BaseEntity
        {
            var entityType = _context.Model.FindEntityType(typeof(T))!;
            var props = entityType.GetProperties().Where(p => !p.IsShadowProperty());

            var originTable = entityType.GetSchemaQualifiedTableName()!;
            var tempTable = $"#Bulk_{typeof(T).Name}_{Guid.NewGuid():N}";

            var connection = (SqlConnection)_context.Database.GetDbConnection();
            var transaction = _context.Database.CurrentTransaction?.GetDbTransaction() as SqlTransaction;

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(ct);
            }

            try
            {
                await CreateTempTebleAsync(connection, transaction, tempTable, originTable, ct);
                var sql = BuildSqlQuery(originTable, tempTable, uniqueIdxProps, props);

                for (int i = 0; i < entities.Count; i += DefaultBatchSize)
                {
                    await ClearTempTableAsync(connection, transaction, tempTable, ct);

                    var batch = entities.GetRange(i, Math.Min(DefaultBatchSize, entities.Count - i));

                    var dt = ToDataTable(batch, props);
                    await BulkCopyAsync(connection, transaction, dt, tempTable, ct);

                    await MergeAndFetchIdsAsync(connection, transaction, sql, batch, ct);
                }
            }
            finally
            {
                await DropTempTableAsync(connection, transaction, tempTable, ct);
            }
        }

        private static async Task MergeAndFetchIdsAsync<T>(SqlConnection conn, SqlTransaction? trans, string sql, List<T> entities, CancellationToken ct) where T : BaseEntity
        {
            using var cmd = new SqlCommand(sql, conn, trans)
            {
                CommandTimeout = DefaultTimeout
            };
            using var reader = await cmd.ExecuteReaderAsync(ct);

            while (await reader.ReadAsync(ct))
            {
                var id = Convert.ToInt64(reader.GetValue(0));
                var idx = reader.GetInt32(1);

                entities[idx].Id = id;
            }
        }

        private static async Task BulkCopyAsync(SqlConnection connection, SqlTransaction? transaction, DataTable dt, string destinationTable, CancellationToken ct)
        {
            using var bcp = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction)
            {
                DestinationTableName = destinationTable,
                BulkCopyTimeout = DefaultTimeout
            };

            foreach (DataColumn column in dt.Columns)
            {
                bcp.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            await bcp.WriteToServerAsync(dt, ct);
        }

        private static async Task CreateTempTebleAsync(SqlConnection connection, SqlTransaction? transaction, string tempTable, string originTable, CancellationToken ct)
        {
            var sql = $@"
                IF OBJECT_ID('tempdb..{tempTable}') IS NOT NULL DROP TABLE {tempTable};
                SELECT TOP 0 * INTO {tempTable} FROM {originTable};
                ALTER TABLE {tempTable} ADD [_Idx] INT;";

            using var cmd = new SqlCommand(sql, connection, transaction);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        private static async Task ClearTempTableAsync(SqlConnection connection, SqlTransaction? transaction, string tempTable, CancellationToken ct)
        {
            var sql = $"TRUNCATE TABLE {tempTable};";

            using var cmd = new SqlCommand(sql, connection, transaction);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        private static async Task DropTempTableAsync(SqlConnection connection, SqlTransaction? transaction, string tempTable, CancellationToken ct)
        {
            var sql = $"DROP TABLE IF EXISTS {tempTable};";

            using var cmd = new SqlCommand(sql, connection, transaction);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        private static DataTable ToDataTable<T>(List<T> entities, IEnumerable<IProperty> props)
        {
            var metadata = GetTypeMetadata<T>(props);
            var dt = new DataTable();

            foreach (var m in metadata)
            {
                dt.Columns.Add(m.ColumnName, m.DbType);
            }
            dt.Columns.Add("_Idx", typeof(int));

            for (int i = 0; i < entities.Count; i++)
            {
                var row = dt.NewRow();
                var entity = entities[i];

                for (int j = 0; j < metadata.Count; j++)
                {
                    var prop = metadata[j];
                    var value = prop.PropertyInfo?.GetValue(entity);

                    if (value == null)
                    {
                        row[prop.ColumnName] = DBNull.Value;
                        continue;
                    }

                    if (prop.Converter != null)
                    {
                        value = prop.Converter!.ConvertToProvider(value);
                    }
                    else if (value is Enum)
                    {
                        value = Convert.ChangeType(value, prop.DbType);
                    }
                    row[prop.ColumnName] = value;
                }
                row["_Idx"] = i;
                dt.Rows.Add(row);
            }

            return dt;
        }

        private static string BuildSqlQuery(string targetTable, string sourceTable, List<string>? uniqueIdxProps, IEnumerable<IProperty> props)
        {
            var sb = new StringBuilder(2048);
            var cols = props.Where(p => !p.IsPrimaryKey()).ToList();

            var colNames = string.Join(',', cols.Select(p => $"[{p.GetColumnName()}]"));
            var sourceCols = string.Join(',', cols.Select(p => $"S.[{p.GetColumnName()}]"));
            var outputClause = "OUTPUT INSERTED.[Id], S.[_Idx]";

            if ((uniqueIdxProps == null) || (uniqueIdxProps.Count == 0))
            {
                sb.Append("INSERT INTO ").Append(targetTable).Append(" (").Append(colNames).Append(") ")
                  .Append(outputClause).Append(" SELECT ").Append(sourceCols).Append($" FROM {sourceTable} AS S");

                return sb.ToString();
            }

            var idxSet = new HashSet<string>(uniqueIdxProps);
            var joinCondition = string.Join(" AND ", uniqueIdxProps.Select(k => $"T.[{k}]=S.[{k}]"));
            var updateSet = string.Join(',', cols
                .Where(p => !idxSet.Contains(p.Name))
                .Select(p => $"T.[{p.GetColumnName()}]=S.[{p.GetColumnName()}]"));

            if (string.IsNullOrEmpty(updateSet))
            {
                updateSet = $"T.[{uniqueIdxProps[0]}]=T.[{uniqueIdxProps[0]}]";
            }

            sb.Append("MERGE ").Append(targetTable).Append($" AS T USING {sourceTable} AS S ON ").Append(joinCondition)
              .AppendLine(" WHEN MATCHED THEN UPDATE SET ").Append(updateSet)
              .AppendLine(" WHEN NOT MATCHED THEN INSERT (").Append(colNames).Append(") VALUES (").Append(sourceCols).Append(')')
              .Append(outputClause).Append(';');

            return sb.ToString();
        }

        private static List<ColumnDefinition> GetTypeMetadata<T>(IEnumerable<IProperty> props)
        {
            return s_columnDefinitions.GetOrAdd(typeof(T), type =>
            {
                return [.. props.Select(p =>
                {
                    var converter = p.GetValueConverter();
                    var dbType = converter?.ProviderClrType;

                    if (dbType == null)
                    {
                        var coreType = Nullable.GetUnderlyingType(p.ClrType) ?? p.ClrType;
                        dbType = coreType.IsEnum ? Enum.GetUnderlyingType(coreType) : coreType;
                    }

                    return new ColumnDefinition(p.GetColumnName(), dbType, type.GetProperty(p.Name), converter);
                })];
            });
        }

        private sealed class BatchDuplicateManager<T>(List<string> uniqueIdxProps) : IEqualityComparer<T> where T : BaseEntity
        {
            private readonly Func<T, T, bool> _equals = CompileEquals(uniqueIdxProps);
            private readonly Func<T, int> _hash = CompileHashCode(uniqueIdxProps);

            public bool Equals(T? x, T? y)
            {
                return (x, y) switch
                {
                    (null, null) => true,
                    (null, _) or (_, null) => false,
                    _ => _equals(x, y)
                };
            }

            public int GetHashCode([DisallowNull] T obj)
            {
                return obj == null ? 0 : _hash(obj);
            }

            public IDictionary<T, List<T>> GroupByUniqueIndex(IEnumerable<T> entities)
            {
                var buckets = new Dictionary<T, List<T>>(this);

                foreach (var entity in entities)
                {
                    if (!buckets.TryGetValue(entity, out var bucket))
                    {
                        bucket = [];
                        buckets.Add(entity, bucket);
                    }

                    bucket.Add(entity);
                }

                return buckets;
            }

            public static void ApplyGeneratedIds(IDictionary<T, List<T>> buckets)
            {
                foreach (var (master, duplicates) in buckets)
                {
                    var masterId = master.Id;

                    for (int i = 1; i < duplicates.Count; i++)
                    {
                        duplicates[i].Id = masterId;
                    }
                }
            }

            private static Func<T, T, bool> CompileEquals(List<string> uniqueIdxProps)
            {
                var x = Expression.Parameter(typeof(T), "x");
                var y = Expression.Parameter(typeof(T), "y");

                var comparsions = uniqueIdxProps.Select<string, Expression>(p =>
                {
                    var propX = Expression.Property(x, p);
                    var propY = Expression.Property(y, p);

                    var comparerType = typeof(EqualityComparer<>).MakeGenericType(propX.Type);
                    var defaultProperty = comparerType.GetProperty("Default", BindingFlags.Public | BindingFlags.Static)!;
                    var defaultComparer = Expression.MakeMemberAccess(null, defaultProperty);

                    return Expression.Call(defaultComparer, "Equals", null, propX, propY);
                });

                var body = comparsions.Aggregate(Expression.AndAlso);

                return Expression.Lambda<Func<T, T, bool>>(body, x, y).Compile();
            }

            private static Func<T, int> CompileHashCode(List<string> uniqueIdxProps)
            {
                var obj = Expression.Parameter(typeof(T), "obj");
                var hashVar = Expression.Variable(typeof(HashCode), "h");

                var addMethodInfo = typeof(HashCode).GetMethods().First(m => (m.Name == nameof(HashCode.Add)) && m.IsGenericMethod);
                var toHashMethod = typeof(HashCode).GetMethod(nameof(HashCode.ToHashCode));

                var body = new List<Expression>
                {
                    Expression.Assign(hashVar, Expression.Default(typeof(HashCode)))
                };

                foreach (var p in uniqueIdxProps)
                {
                    var propAccess = Expression.Property(obj, p);
                    var addMethod = addMethodInfo.MakeGenericMethod(propAccess.Type);

                    body.Add(Expression.Call(hashVar, addMethod!, propAccess));
                }

                body.Add(Expression.Call(hashVar, toHashMethod!));

                return Expression.Lambda<Func<T, int>>(Expression.Block([hashVar], body), obj).Compile();
            }
        }

        private sealed record ColumnDefinition(
            string ColumnName,
            Type DbType,
            PropertyInfo? PropertyInfo,
            ValueConverter? Converter);
    }
}