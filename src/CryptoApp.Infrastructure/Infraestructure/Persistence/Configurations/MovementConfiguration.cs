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

            builder.Property(m => m.user_id).IsRequired().HasMaxLength(50);
            builder.Property(m => m.user_name).HasMaxLength(150);
            builder.Property(m => m.coin_id).IsRequired().HasMaxLength(100);
            builder.Property(m => m.coin_name).HasMaxLength(150);
            builder.Property(m => m.coin_symbol).HasMaxLength(20);
            builder.Property(m => m.type).HasColumnType("char(1)").IsRequired();
            builder.Property(m => m.quantity).HasPrecision(18, 8);
            builder.Property(m => m.currency).HasMaxLength(10);
            builder.Property(m => m.price).HasPrecision(18, 8);
            builder.Property(m => m.total).HasPrecision(18, 2);
            builder.Property(m => m.realized_pnl).HasPrecision(18, 2);
            builder.Property(m => m.created_at).IsRequired();

            // Desnormalizado: sin relaciones FK a otras tablas.
            builder.HasIndex(m => m.user_id);
            builder.HasIndex(m => new { m.user_id, m.coin_id, m.currency });
        }
    }
}
