using MediatR;
using Microsoft.EntityFrameworkCore;
using VibraScan.Application.Common.Interfaces;

namespace VibraScan.Application.Engines.Commands.UpdateEngines
{
    public class UpdateEnginesCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateEnginesCommand>
    {
        private readonly IApplicationDbContext _context = context;

        public async Task Handle(UpdateEnginesCommand request, CancellationToken ct)
        {
            if (!request.EngineIds.Any())
            {
                return;
            }

            await _context.Engines
                .Where(e => request.EngineIds.Contains(e.Id))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(e => e.Condition, request.Condition)
                    .SetProperty(e => e.LastInspectionDate, request.LastInspectionDate)
                    .SetProperty(e => e.NextInspectionDate, e => e.NextInspectionDate),
                ct);
        }
    }
}