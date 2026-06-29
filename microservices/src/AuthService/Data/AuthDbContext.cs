using AuthService.Domain;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<Person> Persons => Set<Person>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("persons");
            entity.HasKey(p => p.id);
            entity.Property(p => p.name).HasMaxLength(150).IsRequired();
            entity.Property(p => p.email).HasMaxLength(254).IsRequired();
            entity.HasIndex(p => p.email).IsUnique();
            entity.Property(p => p.password_hash).IsRequired();
            entity.Property(p => p.role).HasColumnType("char(1)");
        });

        base.OnModelCreating(modelBuilder);
    }
}
