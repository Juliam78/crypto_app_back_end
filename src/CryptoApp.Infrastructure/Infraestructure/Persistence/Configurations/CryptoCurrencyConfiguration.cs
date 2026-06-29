using CryptoAppBackEnd.Infraestructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoAppBackEnd.Infraestructure.Persistence.Configurations
{
    public class CryptoCurrencyConfiguration : IEntityTypeConfiguration<CryptoCurrencyDbModel>
    {
        public void Configure(EntityTypeBuilder<CryptoCurrencyDbModel> builder)
        {
            builder.ToTable("crypto_currencies");

            builder.HasKey(c => c.id);

            builder.Property(c => c.symbol).IsRequired().HasMaxLength(20);
            builder.Property(c => c.name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.image_url).HasMaxLength(500);
            builder.Property(c => c.current_price).HasPrecision(18, 8);
            builder.Property(c => c.price_change_24h).HasPrecision(18, 8);
            builder.Property(c => c.market_cap).HasPrecision(24, 2);
            builder.Property(c => c.last_price_update).IsRequired();
            builder.Property(c => c.created_at).IsRequired();
            builder.Property(c => c.updated_at).IsRequired();

            builder.HasIndex(c => c.symbol).IsUnique();
        }
    }
}
