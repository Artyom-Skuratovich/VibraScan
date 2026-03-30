using VibraScan.Domain.Common;
using VibraScan.Domain.Exceptions;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Domain.Entities
{
    public class Engine : BaseEntity
    {
        public string Name { get; set; } = null!;

        public Condition Condition { get; set; } = Condition.NotChecked;

        public DateTime? LastInspectionDate { get; set; }

        public DateTime? NextInspectionDate { get; set; }

        public long WorkshopId { get; set; }

        public long? InspectionRuleId { get; set; }

        public void ApplyInspection(DateTime inspectionDate, Condition condition, InspectionRule rule)
        {
            var interval = rule.Intervals.FirstOrDefault(i => i.TargetCondition == condition) ??
                throw new InspectionIntervalNotFoundException(rule.Name, condition);

            LastInspectionDate = inspectionDate;
            Condition = condition;
            NextInspectionDate = inspectionDate.AddDays(interval.DaysCount);
            InspectionRuleId = rule.Id;
        }
    }
}