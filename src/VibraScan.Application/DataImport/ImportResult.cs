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

        public ImportError? Error { get; init; }

        public static ImportResult Success(string sourceName) => new()
        {
            IsSuccess = true,
            Message = "Импорт успешно завершён",
            SourceName = sourceName
        };

        public static ImportResult Failure(string message, string error, ImportErrorType errorType, string sourceName, string? stackTrace = null) => new()
        {
            IsSuccess = false,
            Message = message,
            Error = new ImportError(error, errorType, stackTrace),
            SourceName = sourceName
        };
    }
}