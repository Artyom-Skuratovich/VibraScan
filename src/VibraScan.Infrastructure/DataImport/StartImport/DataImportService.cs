using System.Xml;
using VibraScan.Application.DataImport;

namespace VibraScan.Infrastructure.DataImport.StartImport
{
    public class DataImportService(IEnumerable<IEntityProcessor> processors) : IDataImportService
    {
        private readonly Dictionary<string, IEntityProcessor> _processors = processors.ToDictionary(p => p.EntityName);

        public async Task<ImportResult> ImportAsync(Stream stream, IProgress<ImportProgress> progress, CancellationToken ct = default)
        {
            var entCount = new[] { 0 };
            var collCount = 0D;
            var context = new ImportContext();

            try
            {
                using var reader = XmlReader.Create(stream, new XmlReaderSettings
                {
                    CloseInput = false,
                    Async = true,
                    IgnoreComments = true,
                    IgnoreWhitespace = true
                });

                while (await reader.ReadAsync())
                {
                    if (reader is { NodeType: XmlNodeType.Element, Name: "Table" })
                    {
                        var entityName = reader.GetAttribute("name");

                        if (!string.IsNullOrEmpty(entityName) && _processors.TryGetValue(entityName, out var processor))
                        {
                            progress.Report(new ImportProgress(CalculateProgress(collCount), $"Обработка коллекции {entityName}..."));
                            await ProcessCurrentEntitiesAsync(reader, processor, context, entCount, ct);

                            progress.Report(new ImportProgress(CalculateProgress(collCount += 0.8), $"Отправка коллекции {entityName} в БД"));
                            await processor.CommitAsync(context, ct);
                            collCount += 0.2;
                        }
                    }
                }
                progress.Report(new ImportProgress(100, "Импорт успешно завершён"));

                return ImportResult.Success(entCount[0]);
            }
            catch (XmlException ex)
            {
                return ImportResult.Failure("Ошибка структуры XML", ex.Message, ImportErrorType.Parsing, entCount[0]);
            }
            catch (OperationCanceledException)
            {
                return ImportResult.Failure("Импорт отменён", "Операция прервана пользователем", ImportErrorType.Canceled, entCount[0]);
            }
            catch (ImportInconsistencyException ex)
            {
                return ImportResult.Failure("Конфликт данных", ex.Message, ImportErrorType.Inconsistency, entCount[0]);
            }
            catch (Exception ex)
            {
                return ImportResult.Failure("Критическая ошибка", ex.Message, ImportErrorType.Fatal, entCount[0], ex.StackTrace);
            }
        }

        private double CalculateProgress(double collCount)
        {
            var percentage = collCount / _processors.Count * 100;
            return Math.Clamp(percentage, 0, 100);
        }

        private static async Task ProcessCurrentEntitiesAsync(XmlReader reader, IEntityProcessor processor, ImportContext context, int[] entCount, CancellationToken ct)
        {
            using var subReader = reader.ReadSubtree();

            await subReader.ReadAsync();

            while (await subReader.ReadAsync())
            {
                if (subReader is { NodeType: XmlNodeType.Element, Name: "Row" })
                {
                    await processor.ProcessAsync(subReader, context, ct);
                    entCount[0]++;
                }
            }
        }
    }
}