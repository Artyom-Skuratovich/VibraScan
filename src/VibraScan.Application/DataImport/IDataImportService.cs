namespace VibraScan.Application.DataImport
{
    public interface IDataImportService
    {
        Task<ImportResult> ImportAsync(ImportSource source, IProgress<ImportProgress> progress, CancellationToken ct = default);
    }
}