using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VibraScan.Domain.Entities;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Infrastructure.Data.Configurations
{
    internal class VibrationMeasurementConfiguration : IEntityTypeConfiguration<VibrationMeasurement>
    {
        public void Configure(EntityTypeBuilder<VibrationMeasurement> builder)
        {
            builder.Property(m => m.AxisType)
                .HasConversion(a => a.Name, n => AxisType.From(n));

            builder.Property(m => m.MeasurementDomain)
                .HasConversion(d => d.Value, v => MeasurementDomain.From(v));

            builder.HasOne<MeasurementProfile>()
                .WithMany()
                .HasForeignKey(m => m.MeasurementProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Point>()
                .WithMany()
                .HasForeignKey(m => m.PointId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(m => new { m.CapturedAt, m.MeasurementDomain }).IsUnique();

            builder.HasIndex(m => new { m.PointId, m.AxisType, m.MeasurementProfileId, m.CapturedAt });

            builder.HasIndex(m => m.AxisType);
        }
    }
}