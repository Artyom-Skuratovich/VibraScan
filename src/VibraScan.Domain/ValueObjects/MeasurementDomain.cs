using VibraScan.Domain.Common;
using VibraScan.Domain.Exceptions;

namespace VibraScan.Domain.ValueObjects
{
    public class MeasurementDomain : ValueObject
    {
        private MeasurementDomain(int value, string name)
        {
            Value = value;
            Name = name;
        }

        public static MeasurementDomain From(int value)
        {
            var measurementDomain = SupportedMeasurementDomains.FirstOrDefault(d => d.Value == value);
            return measurementDomain ?? throw new UnsupportedValueException(value.ToString(), nameof(MeasurementDomain));
        }

        public static MeasurementDomain Time => new(1, "Временная область");

        public static MeasurementDomain Frequency => new(2, "Частотная область");

        public int Value { get; }

        public string Name { get; }

        public override string? ToString()
        {
            return Name;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        protected static IEnumerable<MeasurementDomain> SupportedMeasurementDomains
        {
            get
            {
                yield return Time;
                yield return Frequency;
            }
        }
    }
}