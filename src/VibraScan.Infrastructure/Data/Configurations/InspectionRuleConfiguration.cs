using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VibraScan.Domain.Entities;

namespace VibraScan.Infrastructure.Data.Configurations
{
    internal class InspectionRuleConfiguration : IEntityTypeConfiguration<InspectionRule>
    {
        public void Configure(EntityTypeBuilder<InspectionRule> builder)
        {
            builder.HasMany(r => r.Intervals)
                .WithOne()
                .HasForeignKey(i => i.InspectionRuleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(r => r.Intervals)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(r => r.Name).IsUnique();
        }
    }
}