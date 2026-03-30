namespace VibraScan.Application.DataImport.Commands.StartBulkImport
{
    public record BulkImportResult
    {
        public IReadOnlyCollection<ImportResult> Details { get; init; } = [];

        public int TotalProcessed { get; init; }

        public bool AllSucceeded { get; init; }
    }
}