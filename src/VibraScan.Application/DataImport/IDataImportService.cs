namespace VibraScan.Application.DataImport
{
    public interface IDataImportService
    {
        Task<ImportResult> ImportAsync(Stream stream, IProgress<ImportProgress> progress, CancellationToken ct = default);
    }
}