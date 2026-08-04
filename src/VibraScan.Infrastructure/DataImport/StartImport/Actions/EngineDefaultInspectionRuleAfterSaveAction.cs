using Microsoft.EntityFrameworkCore;
using VibraScan.Application.Common.Interfaces;
using VibraScan.Domain.Entities;

namespace VibraScan.Infrastructure.DataImport.StartImport.Actions
{
    internal class EngineDefaultInspectionRuleAfterSaveAction(IApplicationDbContext context) : IAfterSaveAction<Engine>
    {
        private readonly IApplicationDbContext _context = context;

        public async Task ExecuteAsync(IEnumerable<Engine> entities, CancellationToken ct = default)
        {
            if (!entities.Any())
            {
                return;
            }

            var defaultRuleId = await _context.InspectionRules
                .Where(r => r.Name == InspectionRule.DefaultName)
                .Select(r => r.Id)
                .FirstOrDefaultAsync(ct);

            var entityIds = entities.Select(e => e.Id).ToList();

            await _context.Engines
                .Where(e => entityIds.Contains(e.Id) && e.InspectionRuleId == null)
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.InspectionRuleId, defaultRuleId), ct);
        }
    }
}