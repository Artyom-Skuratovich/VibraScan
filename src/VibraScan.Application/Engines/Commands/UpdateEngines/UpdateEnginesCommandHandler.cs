using MediatR;
using Microsoft.EntityFrameworkCore;
using VibraScan.Application.Common.Exceptions;
using VibraScan.Application.Common.Interfaces;
using VibraScan.Domain.Entities;
using VibraScan.Domain.ValueObjects;

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

            if (request.IsManualNextDateCalculation)
            {
                if (request.LastInspectionDate.HasValue && request.NextInspectionDate.HasValue)
                {
                    if (request.NextInspectionDate.Value.Date <= request.LastInspectionDate.Value.Date)
                    {
                        throw new ValidationException([new ValidationFailure(
                            nameof(request.NextInspectionDate),
                            "Дата следующей проверки должна быть больше даты последней проверки")]);
                    }
                }
                await UpdateEnginesWithManualDateAsync(request, ct);
            }
            else
            {
                await UpdateEnginesWithAutoCalculatedDateAsync(request, ct);
            }
        }

        private static async Task UpdateEnginesAsync(
            IQueryable<Engine> query,
            Condition condition,
            DateTime? lastInspectionDate,
            DateTime? nextInspectionDate,
            CancellationToken ct)
        {
            await query.ExecuteUpdateAsync(s => s
                .SetProperty(e => e.Condition, condition)
                .SetProperty(e => e.LastInspectionDate, lastInspectionDate)
                .SetProperty(e => e.NextInspectionDate, nextInspectionDate),
            ct);
        }

        private async Task UpdateEnginesWithManualDateAsync(UpdateEnginesCommand request, CancellationToken ct)
        {
            var query = _context.Engines.Where(e => request.EngineIds.Contains(e.Id));
            await UpdateEnginesAsync(query, request.Condition, request.LastInspectionDate, request.NextInspectionDate, ct);
        }

        private async Task UpdateEnginesWithAutoCalculatedDateAsync(UpdateEnginesCommand request, CancellationToken ct)
        {
            if (!request.LastInspectionDate.HasValue)
            {
                return;
            }

            var intervals = await _context.Engines
                .Where(e => request.EngineIds.Contains(e.Id))
                .Select(e => new
                {
                    EngineId = e.Id,
                    DaysCount = _context.InspectionRules
                        .Where(r => r.Id == e.InspectionRuleId)
                        .SelectMany(r => r.Intervals)
                        .Where(i => i.TargetCondition == request.Condition)
                        .Select(i => i.DaysCount)
                        .FirstOrDefault()
                })
                .ToListAsync(ct);

            var groupedIntervals = intervals.GroupBy(x => x.DaysCount);

            foreach (var group in groupedIntervals)
            {
                var targetEngineIds = group.Select(g => g.EngineId).ToList();
                var calculatedNextDate = request.LastInspectionDate.Value.AddDays(group.Key);

                var query = _context.Engines.Where(e => targetEngineIds.Contains(e.Id));
                await UpdateEnginesAsync(query, request.Condition, request.LastInspectionDate, calculatedNextDate, ct);
            }
        }
    }
}