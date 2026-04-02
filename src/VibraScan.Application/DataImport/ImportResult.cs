namespace VibraScan.Application.DataImport
{
    public enum ImportErrorType
    {
        Parsing,
        Inconsistency,
        Canceled,
        Fatal
    }

    public record ImportError(string Message, ImportErrorType Type, string? StackTrace = null);

    public record ImportResult
    {
        public bool IsSuccess { get; init; }

        public string Message { get; init; } = string.Empty;

        public string SourceName { get; init; } = string.Empty;

        public int ProcessedEntitiesCount { get; init; }

        public ImportError? Error { get; init; }

        public static ImportResult Success(int count, string sourceName)
        {
            return new ImportResult
            {
                IsSuccess = true,
                Message = "Импорт успешно завершён",
                ProcessedEntitiesCount = count,
                SourceName = sourceName
            };
        }

        public static ImportResult Failure(string summary, string detail, ImportErrorType errorType, int processedBeforeError, string sourceName, string? stackTrace = null)
        {
            return new ImportResult
            {
                IsSuccess = false,
                Message = summary,
                ProcessedEntitiesCount = processedBeforeError,
                Error = new ImportError(detail, errorType, stackTrace),
                SourceName = sourceName
            };
        }
    }
}