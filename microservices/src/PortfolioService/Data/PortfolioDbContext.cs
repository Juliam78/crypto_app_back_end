using Microsoft.EntityFrameworkCore;

namespace PortfolioService.Data;

public class PortfolioDbContext : DbContext
{
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options) { }

    public DbSet<Portfolio> Portfolios => Set<Portfolio>();
    public DbSet<PortfolioAsset> PortfolioAssets => Set<PortfolioAsset>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Portfolio>(entity =>
        {
            entity.ToTable("portfolios");
            entity.HasKey(p => p.id);
            entity.Property(p => p.name).HasMaxLength(150).IsRequired();
            entity.Property(p => p.base_currency).HasMaxLength(10).IsRequired();
            entity.HasMany(p => p.assets)
                  .WithOne()
                  .HasForeignKey(a => a.portfolio_id)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PortfolioAsset>(entity =>
        {
            entity.ToTable("portfolio_assets");
            entity.HasKey(a => a.id);
            entity.Property(a => a.coin_id).HasMaxLength(100).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
