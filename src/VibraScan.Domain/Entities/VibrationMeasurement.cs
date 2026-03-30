using VibraScan.Domain.Common;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Domain.Entities
{
    public class VibrationMeasurement : BaseEntity
    {
        public AxisType AxisType { get; set; } = null!;

        public MeasurementDomain MeasurementDomain { get; set; } = null!;

        public DateTime CapturedAt { get; set; }

        public byte[] RawData { get; set; } = [];

        public float Rms { get; set; }

        public long PointId { get; set; }

        public long MeasurementProfileId { get; set; }
    }
}