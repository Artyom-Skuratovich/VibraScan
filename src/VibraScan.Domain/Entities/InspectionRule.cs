using VibraScan.Domain.Common;
using VibraScan.Domain.Exceptions;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Domain.Entities
{
    public class InspectionRule : BaseEntity
    {
        public const string DefaultName = "DEFAULT";

        public string Name { get; set; } = null!;

        private readonly List<InspectionInterval> _intervals = [];
        public IReadOnlyCollection<InspectionInterval> Intervals => _intervals.AsReadOnly();

        public void AddInterval(Condition condition, int daysCount)
        {
            if (_intervals.Any(i => i.TargetCondition == condition))
            {
                throw new DuplicateInspectionIntervalException(condition, Name);
            }

            _intervals.Add(new InspectionInterval
            {
                TargetCondition = condition,
                DaysCount = daysCount
            });
        }
    }
}