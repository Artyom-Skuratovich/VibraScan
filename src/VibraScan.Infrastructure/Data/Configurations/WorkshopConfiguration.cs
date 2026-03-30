using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VibraScan.Domain.Entities;

namespace VibraScan.Infrastructure.Data.Configurations
{
    internal class WorkshopConfiguration : IEntityTypeConfiguration<Workshop>
    {
        public void Configure(EntityTypeBuilder<Workshop> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Id)
                .ValueGeneratedOnAdd();

            builder.HasIndex(w => w.Name).IsUnique();
        }
    }
}