using MediatR;
using VibraScan.Application.Common.Interfaces;

namespace VibraScan.Application.DataImport.Commands.StartImport
{
    public class StartImportCommandHandler(IApplicationDbContext context, IDataImportService service) : IRequestHandler<StartImportCommand, ImportResult>
    {
        private readonly IApplicationDbContext _context = context;
        private readonly IDataImportService _service = service;

        public async Task<ImportResult> Handle(StartImportCommand request, CancellationToken ct)
        {
            int processedEntitiesCount = 0;

            try
            {
                await _context.BeginTransactionAsync(ct);

                var result = await _service.ImportAsync(request.Source, request.Progress, ct);
                processedEntitiesCount = result.ProcessedEntitiesCount;

                if (result.IsSuccess)
                {
                    await _context.CommitTransactionAsync(ct);
                }
                else
                {
                    await _context.RollbackTransactionAsync(ct);
                }

                return result;
            }
            catch (Exception ex)
            {
                return ImportResult.Failure("Сбой транзакции при импорте данных", ex.Message, ImportErrorType.Fatal, processedEntitiesCount, request.Source.Name, ex.StackTrace);
            }
        }
    }
}