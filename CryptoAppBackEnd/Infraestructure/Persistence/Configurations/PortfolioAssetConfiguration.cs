using CryptoAppBackEnd.Domains.Entities.CryptoCurrencies;
using CryptoAppBackEnd.Domains.Entities.Portfolios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoAppBackEnd.Infraestructure.Persistence.Configurations
{
    public class PortfolioAssetConfiguration : IEntityTypeConfiguration<PortfolioAsset>
    {
        public void Configure(EntityTypeBuilder<PortfolioAsset> builder)
        {
            builder.ToTable("portfolio_assets");

            builder.HasKey(a => a.id);

            builder.Property(a => a.portfolio_id).IsRequired();
            builder.Property(a => a.crypto_id).IsRequired();
            builder.Property(a => a.quantity).HasPrecision(18, 8);
            builder.Property(a => a.average_buy_price).HasPrecision(18, 8);
            builder.Property(a => a.total_invested).HasPrecision(18, 2);
            builder.Property(a => a.created_at).IsRequired();
            builder.Property(a => a.updated_at).IsRequired();

            builder.HasOne<Portfolio>()
                .WithMany()
                .HasForeignKey(a => a.portfolio_id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<CryptoCurrency>()
                .WithMany()
                .HasForeignKey(a => a.crypto_id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
