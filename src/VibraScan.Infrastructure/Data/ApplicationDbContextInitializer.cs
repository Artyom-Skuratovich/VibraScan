using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VibraScan.Domain.Entities;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Infrastructure.Data
{
    public static class InitializerExtensions
    {
        public static async Task InitializeDatabaseAsync(this IServiceScopeFactory scopeFactory, CancellationToken ct)
        {
            using var scope = scopeFactory.CreateScope();
            var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();

            await initializer.InitializeAsync(ct);
            await initializer.SeedAsync(ct);
        }
    }

    public class ApplicationDbContextInitializer(ApplicationDbContext context)
    {
        private readonly ApplicationDbContext _context = context;

        public async Task InitializeAsync(CancellationToken ct)
        {
            await _context.Database.MigrateAsync(ct);
        }

        public async Task SeedAsync(CancellationToken ct)
        {
            var defaultRule = await _context.InspectionRules
                .Include(r => r.Intervals)
                .FirstOrDefaultAsync(r => r.Name == InspectionRule.DefaultName, ct);

            if (defaultRule == null)
            {
                defaultRule = new InspectionRule
                {
                    Name = InspectionRule.DefaultName
                };

                foreach (var interval in GetDefaultIntervals())
                {
                    defaultRule.AddInterval(interval.TargetCondition, interval.DaysCount);
                }

                _context.InspectionRules.Add(defaultRule);
            }
            else
            {
                var expectedIntervals = GetDefaultIntervals();

                foreach (var expected in expectedIntervals)
                {
                    var existingInterval = defaultRule.Intervals
                        .FirstOrDefault(i => i.TargetCondition == expected.TargetCondition);

                    if (existingInterval == null)
                    {
                        defaultRule.AddInterval(expected.TargetCondition, expected.DaysCount);
                    }
                    else if (existingInterval.DaysCount != expected.DaysCount)
                    {
                        existingInterval.DaysCount = expected.DaysCount;
                    }
                }
            }

            await _context.SaveChangesAsync(ct);
        }

        private static List<InspectionInterval> GetDefaultIntervals()
        {
            return
            [
                new() { TargetCondition = Condition.Excellent, DaysCount = 180 },
                new() { TargetCondition = Condition.Satisfactory, DaysCount = 30 },
                new() { TargetCondition = Condition.Unsatisfactory, DaysCount = 7 },
                new() { TargetCondition = Condition.Bad, DaysCount = 1 },
                new() { TargetCondition = Condition.NotChecked, DaysCount = 14 }
            ];
        }
    }
}