using VibraScan.Domain.ValueObjects;

namespace VibraScan.Infrastructure.DataImport.StartImport.Models
{
    internal record ProxySchedule
    {
        public AxisType AxisType { get; init; } = null!;

        public long PointId { get; init; }

        public long MeasurementProfileId { get; init; }
    }
}