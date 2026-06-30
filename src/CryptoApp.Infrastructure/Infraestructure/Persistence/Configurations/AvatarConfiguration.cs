using CryptoAppBackEnd.Infraestructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoAppBackEnd.Infraestructure.Persistence.Configurations
{
    public class AvatarConfiguration : IEntityTypeConfiguration<AvatarDbModel>
    {
        public void Configure(EntityTypeBuilder<AvatarDbModel> builder)
        {
            builder.ToTable("avatars");

            builder.HasKey(a => a.person_id);

            // person_id es la PK provista por la app (no autogenerada).
            builder.Property(a => a.person_id).ValueGeneratedNever();
            builder.Property(a => a.data).IsRequired();
            builder.Property(a => a.content_type).IsRequired().HasMaxLength(100);
            builder.Property(a => a.updated_at).IsRequired();
        }
    }
}
