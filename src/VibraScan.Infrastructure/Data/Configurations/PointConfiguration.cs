using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VibraScan.Domain.Entities;

namespace VibraScan.Infrastructure.Data.Configurations
{
    internal class PointConfiguration : IEntityTypeConfiguration<Point>
    {
        public void Configure(EntityTypeBuilder<Point> builder)
        {
            builder.HasOne<Engine>()
                .WithMany()
                .HasForeignKey(p => p.EngineId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => new { p.EngineId, p.Name }).IsUnique();
        }
    }
}