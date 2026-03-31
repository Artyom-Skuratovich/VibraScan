using VibraScan.Application.DataImport;
using VibraScan.Application.DataImport.Commands.StartBulkImport;

namespace VibraScan.WinForms.Views.Abstractions
{
    public interface IDataImportView : IView
    {
        event EventHandler? CancelRequested;

        void PrepareForImport(int totalFiles);

        void UpdateFileProgress(double percentage, string stage);

        void UpdateOverallProgress(int processedCount, string description);

        void ShowResults(ImportResult result);

        void ShowResults(BulkImportResult result);

        void SetCancelable(bool canCancel);
    }
}