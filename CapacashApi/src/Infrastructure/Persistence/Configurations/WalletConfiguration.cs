using Capacash.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Capacash.Infrastructure.Persistence.Configuration
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.ToTable("Wallets");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.UserId).IsRequired();
            builder.Property(w => w.Balance).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(w => w.CreatedAt).IsRequired();
            builder.Property(w => w.UpdatedAt);

            builder.HasIndex(w => w.UserId).IsUnique();
        }
    }
}
