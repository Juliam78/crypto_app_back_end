using CryptoAppBackEnd.Domains.Entities.Persons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoAppBackEnd.Infraestructure.Persistence.Configurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("persons");

            builder.HasKey(p => p.id);

            builder.Property(p => p.name).IsRequired().HasMaxLength(150);
            builder.Property(p => p.email).IsRequired().HasMaxLength(254);
            builder.Property(p => p.password_hash).IsRequired();
            builder.Property(p => p.role).IsRequired();
            builder.Property(p => p.status).IsRequired();
            builder.Property(p => p.created_at).IsRequired();
            builder.Property(p => p.updated_at).IsRequired();

            builder.HasIndex(p => p.email).IsUnique();
        }
    }
}
