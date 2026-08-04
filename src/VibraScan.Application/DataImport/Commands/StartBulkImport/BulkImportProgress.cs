namespace VibraScan.Application.DataImport.Commands.StartBulkImport
{
    public readonly struct BulkImportProgress(int currentIndex, int totalCount, string description, ImportError? error = null)
    {
        public int CurrentNumber { get; } = currentIndex;

        public int TotalCount { get; } = totalCount;

        public string Description { get; } = description;

        public ImportError? Error { get; } = error;
    }
}