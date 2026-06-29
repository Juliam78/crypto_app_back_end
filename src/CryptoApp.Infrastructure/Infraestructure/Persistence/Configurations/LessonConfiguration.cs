using CryptoAppBackEnd.Infraestructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoAppBackEnd.Infraestructure.Persistence.Configurations
{
    public class LessonConfiguration : IEntityTypeConfiguration<LessonDbModel>
    {
        public void Configure(EntityTypeBuilder<LessonDbModel> builder)
        {
            builder.ToTable("lessons");

            builder.HasKey(l => l.id);

            builder.Property(l => l.kind).HasColumnType("char(1)").IsRequired();
            builder.Property(l => l.title).IsRequired().HasMaxLength(200);
            builder.Property(l => l.body).IsRequired();
            builder.Property(l => l.coin_id).HasMaxLength(100);
            builder.Property(l => l.coin_symbol).HasMaxLength(20);
            builder.Property(l => l.recommendation).HasColumnType("char(1)");
            builder.Property(l => l.author_id).IsRequired().HasMaxLength(50);
            builder.Property(l => l.author_name).HasMaxLength(150);
            builder.Property(l => l.published).IsRequired();
            builder.Property(l => l.created_at).IsRequired();
            builder.Property(l => l.updated_at).IsRequired();

            // Filtrados frecuentes: por tipo y por estado de publicación.
            builder.HasIndex(l => l.kind);
            builder.HasIndex(l => l.published);
        }
    }
}
