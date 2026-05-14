using MediatR;
using VibraScan.Application.DataImport.Commands.StartImport;

namespace VibraScan.Application.DataImport.Commands.StartBulkImport
{
    public class StartBulkImportCommandHandler(IMediator mediator) : IRequestHandler<StartBulkImportCommand, BulkImportResult>
    {
        private readonly IMediator _mediator = mediator;

        public async Task<BulkImportResult> Handle(StartBulkImportCommand request, CancellationToken ct)
        {
            var sources = request.Sources.ToList();
            var details = new List<ImportResult>(sources.Count);
            var currentIndex = 0;

            for (int i = 0; i < sources.Count; i++)
            {
                if (ct.IsCancellationRequested) break;

                currentIndex = i + 1;
                request.OverallProgress.Report(new BulkImportProgress(currentIndex, sources.Count, $"Обработка данных источника: '{sources[i].Name}'"));

                var result = await _mediator.Send(new StartImportCommand
                {
                    Source = sources[i],
                    Progress = request.SegmentProgress,
                }, ct);

                details.Add(result);
            }

            request.OverallProgress.Report(new BulkImportProgress(currentIndex, sources.Count, "Обработка данных завершена"));

            return new BulkImportResult
            {
                Details = details.AsReadOnly(),
                AllSucceeded = !details.Any(d => !d.IsSuccess)
            };
        }
    }
}