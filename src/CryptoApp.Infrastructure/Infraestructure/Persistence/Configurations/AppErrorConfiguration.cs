using CryptoAppBackEnd.Infraestructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoAppBackEnd.Infraestructure.Persistence.Configurations
{
    public class AppErrorConfiguration : IEntityTypeConfiguration<AppErrorDbModel>
    {
        public void Configure(EntityTypeBuilder<AppErrorDbModel> builder)
        {
            builder.ToTable("app_errors");

            builder.HasKey(e => e.id);

            builder.Property(e => e.route).IsRequired().HasMaxLength(500);
            builder.Property(e => e.message).IsRequired();
            builder.Property(e => e.stack);
            builder.Property(e => e.user_id).HasMaxLength(50);
            builder.Property(e => e.user_email).HasMaxLength(254);
            builder.Property(e => e.created_at).IsRequired();

            builder.HasIndex(e => e.created_at);
        }
    }
}
