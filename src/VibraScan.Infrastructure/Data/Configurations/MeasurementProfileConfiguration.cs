using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VibraScan.Domain.Entities;

namespace VibraScan.Infrastructure.Data.Configurations
{
    internal class MeasurementProfileConfiguration : IEntityTypeConfiguration<MeasurementProfile>
    {
        public void Configure(EntityTypeBuilder<MeasurementProfile> builder)
        {
            builder.HasIndex(p => p.Description).IsUnique();
        }
    }
}