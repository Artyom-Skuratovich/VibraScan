using VibraScan.Domain.Common;
using VibraScan.Domain.Exceptions;

namespace VibraScan.Domain.ValueObjects
{
    public class Condition : ValueObject
    {
        private Condition(int value, string description, int priority)
        {
            Value = value;
            Description = description;
            Priority = priority;
        }

        public static Condition From(int value)
        {
            var condition = SupportedConditions.FirstOrDefault(c => c.Value == value);
            return condition ?? throw new UnsupportedValueException(value.ToString(), nameof(Condition));
        }

        public static Condition Excellent => new(1, "Отлично", 1);

        public static Condition Satisfactory => new(2, "Удовлетворительно", 2);

        public static Condition Unsatisfactory => new(3, "Не удовлетворительно", 3);

        public static Condition Bad => new(4, "Плохо", 4);

        public static Condition NotChecked => new(5, "Не удалось проверить", 5);

        public int Value { get; }

        public string Description { get; }

        public int Priority { get; }

        public override string? ToString()
        {
            return Description;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        protected static IEnumerable<Condition> SupportedConditions
        {
            get
            {
                yield return Excellent;
                yield return Satisfactory;
                yield return Unsatisfactory;
                yield return Bad;
                yield return NotChecked;
            }
        }
    }
}