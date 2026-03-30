using VibraScan.Domain.Common;
using VibraScan.Domain.Exceptions;

namespace VibraScan.Domain.ValueObjects
{
    public class AxisType : ValueObject
    {
        private AxisType(string name)
        {
            Name = name;
        }

        public static AxisType From(string name)
        {
            var axisType = SupportedAxisTypes.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return axisType ?? throw new UnsupportedValueException(name, nameof(AxisType));
        }

        public static AxisType Vertical => new("Vertical");

        public static AxisType Horizontal => new("Horizontal");

        public static AxisType Axial => new("Axial");

        public string Name { get; }

        public override string? ToString()
        {
            return Name;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }

        protected static IEnumerable<AxisType> SupportedAxisTypes
        {
            get
            {
                yield return Vertical;
                yield return Horizontal;
                yield return Axial;
            }
        }
    }
}