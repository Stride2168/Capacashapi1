using Capacash.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Capacash.Infrastructure.Persistence.Configuration
{
    public class KioskConfiguration : IEntityTypeConfiguration<Kiosk>
    {
        public void Configure(EntityTypeBuilder<Kiosk> builder)
        {
            builder.ToTable("Kiosks");

            builder.HasKey(k => k.Id);
            builder.Property(k => k.KioskId).IsRequired().HasMaxLength(50);
            builder.Property(k => k.PasswordHash).IsRequired();
            builder.Property(k => k.Name).HasMaxLength(100);
            builder.Property(k => k.Location).HasMaxLength(100);
            builder.Property(k => k.CompanyId).IsRequired();

            builder.HasIndex(k => k.KioskId).IsUnique();
        }
    }
}