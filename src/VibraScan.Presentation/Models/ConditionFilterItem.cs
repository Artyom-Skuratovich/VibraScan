using VibraScan.Domain.ValueObjects;

namespace VibraScan.Presentation.Models
{
    public class ConditionFilterItem(Condition? condition)
    {

        public Condition? Value { get; } = condition;

        public static IEnumerable<ConditionFilterItem> AvailableConditions { get; } = [
            new(null),
            new(Condition.Excellent),
            new(Condition.Satisfactory),
            new(Condition.Unsatisfactory),
            new(Condition.Bad),
            new(Condition.NotChecked)
        ];

        public override string ToString()
        {
            return Value?.Description ?? "Все состояния";
        }
    }
}