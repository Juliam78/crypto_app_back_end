using CryptoAppBackEnd.Infraestructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoAppBackEnd.Infraestructure.Persistence.Configurations
{
    public class MovementConfiguration : IEntityTypeConfiguration<MovementDbModel>
    {
        public void Configure(EntityTypeBuilder<MovementDbModel> builder)
        {
            builder.ToTable("movements");

            builder.HasKey(m => m.id);

            builder.Property(m => m.person_id).IsRequired();
            builder.Property(m => m.portfolio_id).IsRequired();
            builder.Property(m => m.crypto_id).IsRequired();
            builder.Property(m => m.type).IsRequired();
            builder.Property(m => m.quantity).HasPrecision(18, 8);
            builder.Property(m => m.price).HasPrecision(18, 8);
            builder.Property(m => m.total).HasPrecision(18, 2);
            builder.Property(m => m.realized_pnl).HasPrecision(18, 2);
            builder.Property(m => m.created_at).IsRequired();

            builder.HasOne<PersonDbModel>()
                .WithMany()
                .HasForeignKey(m => m.person_id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<PortfolioDbModel>()
                .WithMany()
                .HasForeignKey(m => m.portfolio_id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<CryptoCurrencyDbModel>()
                .WithMany()
                .HasForeignKey(m => m.crypto_id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
