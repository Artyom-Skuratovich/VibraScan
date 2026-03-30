using VibraScan.Domain.Common;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Domain.Entities
{
    public class InspectionInterval : BaseEntity
    {
        public Condition TargetCondition { get; set; } = null!;

        public int DaysCount { get; set; }

        public long InspectionRuleId { get; set; }
    }
}