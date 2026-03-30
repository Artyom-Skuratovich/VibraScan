using System.Xml;

namespace VibraScan.Infrastructure.DataImport.StartImport.EntityProcessors.Base
{
    internal abstract class AbstractEntityProcessor<T> : IEntityProcessor where T : class
    {
        public abstract string EntityName { get; }

        public async Task ProcessAsync(XmlReader reader, ImportContext context, CancellationToken ct = default)
        {
            var fields = new Dictionary<string, string>(StringComparer.Ordinal);
            await ReadFieldsAsync(reader, (name, value) => fields[name] = value, ct);

            if (!TryMap(fields, out var rawId, out var entity, context))
            {
                throw new XmlException($"Некорректная структура записи '{EntityName}'. Отсутствуют обязательные поля");
            }

            OnProcessed(rawId!, entity!, context);
        }

        public abstract Task CommitAsync(ImportContext context, CancellationToken ct = default);

        protected abstract bool TryMap(IReadOnlyDictionary<string, string> data, out string? rawId, out T? entity, ImportContext context);

        protected abstract void OnProcessed(string rawId, T entity, ImportContext context);

        private static async Task ReadFieldsAsync(XmlReader reader, Action<string, string> onFieldFound, CancellationToken ct)
        {
            if ((reader.NodeType != XmlNodeType.Element) || reader.IsEmptyElement) return;

            var depth = reader.Depth;

            if (!await reader.ReadAsync()) return;

            while (reader.Depth > depth)
            {
                ct.ThrowIfCancellationRequested();

                if (reader.NodeType == XmlNodeType.Element)
                {
                    var name = reader.Name;
                    var value = await reader.ReadElementContentAsStringAsync();

                    onFieldFound(name, value);
                    continue;
                }

                if (!await reader.ReadAsync()) break;
            }
        }
    }
}