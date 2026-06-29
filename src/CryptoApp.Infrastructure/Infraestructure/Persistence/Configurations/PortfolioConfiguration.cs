using CryptoAppBackEnd.Infraestructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoAppBackEnd.Infraestructure.Persistence.Configurations
{
    public class PortfolioConfiguration : IEntityTypeConfiguration<PortfolioDbModel>
    {
        public void Configure(EntityTypeBuilder<PortfolioDbModel> builder)
        {
            builder.ToTable("portfolios");

            builder.HasKey(p => p.id);

            builder.Property(p => p.person_id).IsRequired();
            builder.Property(p => p.name).IsRequired().HasMaxLength(150);
            builder.Property(p => p.base_currency).IsRequired().HasMaxLength(10);
            builder.Property(p => p.created_at).IsRequired();
            builder.Property(p => p.updated_at).IsRequired();

            builder.HasOne<PersonDbModel>()
                .WithMany()
                .HasForeignKey(p => p.person_id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
