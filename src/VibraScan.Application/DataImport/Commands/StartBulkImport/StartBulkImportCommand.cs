using MediatR;

namespace VibraScan.Application.DataImport.Commands.StartBulkImport
{
    public record StartBulkImportCommand : IRequest<BulkImportResult>
    {
        public IEnumerable<Stream> DataStreams { get; init; } = [];

        public IProgress<ImportProgress> SegmentProgress { get; init; } = null!;

        public IProgress<BulkImportProgress> OverallProgress { get; init; } = null!;
    }
}