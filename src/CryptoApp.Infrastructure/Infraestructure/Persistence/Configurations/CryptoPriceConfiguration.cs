using CryptoAppBackEnd.Infraestructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoAppBackEnd.Infraestructure.Persistence.Configurations
{
    public class CryptoPriceConfiguration : IEntityTypeConfiguration<CryptoPriceDbModel>
    {
        public void Configure(EntityTypeBuilder<CryptoPriceDbModel> builder)
        {
            builder.ToTable("crypto_prices");

            builder.HasKey(c => c.id);

            builder.Property(c => c.currency).IsRequired().HasMaxLength(10);
            builder.Property(c => c.payload).IsRequired().HasColumnType("text");
            builder.Property(c => c.last_updated).IsRequired();

            builder.HasIndex(c => c.currency).IsUnique();
        }
    }
}
