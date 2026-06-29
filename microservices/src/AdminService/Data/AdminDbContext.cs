using Microsoft.EntityFrameworkCore;

namespace AdminService.Data;

public class AdminDbContext : DbContext
{
    public AdminDbContext(DbContextOptions<AdminDbContext> options) : base(options) { }

    public DbSet<AppError> AppErrors => Set<AppError>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppError>(entity =>
        {
            entity.ToTable("app_errors");
            entity.HasKey(e => e.id);
            entity.Property(e => e.route).HasMaxLength(300);
            entity.HasIndex(e => e.created_at);
        });

        base.OnModelCreating(modelBuilder);
    }
}
