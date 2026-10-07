namespace VibraScan.Application.Reports.Commands
{
    public interface IReportStorageService
    {
        Task SaveReportAsync(ReportData data, string destinationPath, CancellationToken ct = default);
    }
}