using CryptoAppBackEnd.Infraestructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoAppBackEnd.Infraestructure.Persistence.Configurations
{
    public class PortfolioAssetConfiguration : IEntityTypeConfiguration<PortfolioAssetDbModel>
    {
        public void Configure(EntityTypeBuilder<PortfolioAssetDbModel> builder)
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

            builder.HasOne<PortfolioDbModel>()
                .WithMany()
                .HasForeignKey(a => a.portfolio_id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<CryptoCurrencyDbModel>()
                .WithMany()
                .HasForeignKey(a => a.crypto_id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
