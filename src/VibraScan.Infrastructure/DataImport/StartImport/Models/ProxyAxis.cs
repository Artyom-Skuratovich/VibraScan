using VibraScan.Domain.ValueObjects;

namespace VibraScan.Infrastructure.DataImport.StartImport.Models
{
    internal record ProxyAxis
    {
        public AxisType AxisType { get; init; } = null!;

        public long PointId { get; init; }
    }
}