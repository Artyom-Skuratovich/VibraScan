namespace VibraScan.Application.DataImport.Commands.StartBulkImport
{
    public readonly struct BulkImportProgress(int currentIndex, int totalCount, string description)
    {
        public int CurrentIndex { get; } = currentIndex;

        public int TotalCount { get; } = totalCount;

        public string Description { get; } = description;
    }
}