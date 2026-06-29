using Microsoft.EntityFrameworkCore;

namespace TradingService.Data;

public class TradingDbContext : DbContext
{
    public TradingDbContext(DbContextOptions<TradingDbContext> options) : base(options) { }

    public DbSet<Movement> Movements => Set<Movement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movement>(entity =>
        {
            entity.ToTable("movements");
            entity.HasKey(m => m.id);
            entity.Property(m => m.user_id).HasMaxLength(50).IsRequired();
            entity.Property(m => m.coin_id).HasMaxLength(100).IsRequired();
            entity.Property(m => m.coin_symbol).HasMaxLength(20);
            entity.Property(m => m.currency).HasMaxLength(10);
            entity.Property(m => m.type).HasColumnType("char(1)");
            entity.HasIndex(m => m.user_id);
        });

        base.OnModelCreating(modelBuilder);
    }
}
