using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VibraScan.Domain.Entities;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Infrastructure.Data.Configurations
{
    internal class InspectionIntervalConfiguration : IEntityTypeConfiguration<InspectionInterval>
    {
        public void Configure(EntityTypeBuilder<InspectionInterval> builder)
        {
            builder.Property(i => i.TargetCondition)
                .HasConversion(c => c.Value, v => Condition.From(v));

            builder.HasIndex(i => new { i.InspectionRuleId, i.TargetCondition }).IsUnique();
        }
    }
}