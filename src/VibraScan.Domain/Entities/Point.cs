using VibraScan.Domain.Common;

namespace VibraScan.Domain.Entities
{
    public class Point : BaseEntity
    {
        public string Name { get; set; } = null!;

        public long EngineId { get; set; }
    }
}