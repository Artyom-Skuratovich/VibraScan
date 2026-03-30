using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VibraScan.Domain.Entities;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Infrastructure.Data.Configurations
{
    internal class EngineConfiguration : IEntityTypeConfiguration<Engine>
    {
        public void Configure(EntityTypeBuilder<Engine> builder)
        {
            builder.Property(e => e.Condition)
                   .HasConversion(c => c.Value, v => Condition.From(v));

            builder.HasOne<Workshop>()
                .WithMany()
                .HasForeignKey(e => e.WorkshopId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<InspectionRule>()
                .WithMany()
                .HasForeignKey(e => e.InspectionRuleId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(e => new { e.WorkshopId, e.Name }).IsUnique();
        }
    }
}