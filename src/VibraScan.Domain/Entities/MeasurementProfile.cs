using VibraScan.Domain.Common;

namespace VibraScan.Domain.Entities
{
    public class MeasurementProfile : BaseEntity
    {
        public string Description { get; set; } = null!;
    }
}