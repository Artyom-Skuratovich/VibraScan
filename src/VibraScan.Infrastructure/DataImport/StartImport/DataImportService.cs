using System.Xml;
using VibraScan.Application.DataImport;

namespace VibraScan.Infrastructure.DataImport.StartImport
{
    public class DataImportService(IEnumerable<IEntityProcessor> processors) : IDataImportService
    {
        private readonly Dictionary<string, IEntityProcessor> _processors = processors.ToDictionary(p => p.EntityName);

        private const double ProcessingRatio = 0.8;
        private const double CommitRatio = 0.2;

        public async Task<ImportResult> ImportAsync(ImportSource source, IProgress<ImportProgress> progress, CancellationToken ct = default)
        {
            var collCount = 0.0;
            var context = new ImportContext();

            try
            {
                using var reader = XmlReader.Create(source.Data, new XmlReaderSettings
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
                            await ProcessCurrentEntitiesAsync(reader, processor, context, ct);

                            collCount += ProcessingRatio;
                            progress.Report(new ImportProgress(CalculateProgress(collCount), $"Отправка коллекции {entityName} в БД"));

                            await processor.CommitAsync(context, ct);
                            collCount += CommitRatio;
                        }
                    }
                }
                progress.Report(new ImportProgress(100, "Импорт успешно завершён"));

                return ImportResult.Success(source.Name);
            }
            catch (XmlException ex)
            {
                return ImportResult.Failure("Ошибка структуры XML", ex.Message, ImportErrorType.Parsing, source.Name);
            }
            catch (OperationCanceledException)
            {
                return ImportResult.Failure("Импорт отменён", "Операция прервана пользователем", ImportErrorType.Canceled, source.Name);
            }
            catch (ImportInconsistencyException ex)
            {
                return ImportResult.Failure("Конфликт данных", ex.Message, ImportErrorType.Inconsistency, source.Name);
            }
            catch (Exception ex)
            {
                return ImportResult.Failure("Критическая ошибка", ex.Message, ImportErrorType.Fatal, source.Name, ex.StackTrace);
            }
        }

        private double CalculateProgress(double collCount)
        {
            var percentage = collCount / _processors.Count * 100;
            return Math.Clamp(percentage, 0, 100);
        }

        private static async Task ProcessCurrentEntitiesAsync(XmlReader reader, IEntityProcessor processor, ImportContext context, CancellationToken ct)
        {
            using var subReader = reader.ReadSubtree();

            await subReader.ReadAsync();

            while (await subReader.ReadAsync())
            {
                if (subReader is { NodeType: XmlNodeType.Element, Name: "Row" })
                {
                    await processor.ProcessAsync(subReader, context, ct);
                }
            }
        }
    }
}