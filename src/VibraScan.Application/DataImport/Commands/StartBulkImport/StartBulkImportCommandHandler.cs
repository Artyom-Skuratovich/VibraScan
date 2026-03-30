using MediatR;
using VibraScan.Application.DataImport.Commands.StartImport;

namespace VibraScan.Application.DataImport.Commands.StartBulkImport
{
    public class StartBulkImportCommandHandler(IMediator mediator) : IRequestHandler<StartBulkImportCommand, BulkImportResult>
    {
        private readonly IMediator _mediator = mediator;

        public async Task<BulkImportResult> Handle(StartBulkImportCommand request, CancellationToken ct)
        {
            var streams = request.DataStreams.ToList();
            var details = new List<ImportResult>(streams.Count);
            var processedCount = 0;

            for (int i = 0; i < streams.Count; i++)
            {
                if (ct.IsCancellationRequested) break;

                processedCount = i + 1;
                request.OverallProgress.Report(new BulkImportProgress(processedCount, streams.Count, $"Обработка данных: {processedCount} из {streams.Count}"));

                var result = await _mediator.Send(new StartImportCommand
                {
                    DataStream = streams[i],
                    Progress = request.SegmentProgress,
                }, ct);

                details.Add(result);
            }

            request.OverallProgress.Report(new BulkImportProgress(processedCount, streams.Count, "Обработка данных завершена"));

            return new BulkImportResult
            {
                Details = details.AsReadOnly(),
                TotalProcessed = details.Sum(d => d.ProcessedEntitiesCount),
                AllSucceeded = !details.Any(d => !d.IsSuccess)
            };
        }
    }
}