using Microsoft.EntityFrameworkCore;

namespace MarketService.Data;

public class MarketDbContext : DbContext
{
    public MarketDbContext(DbContextOptions<MarketDbContext> options) : base(options) { }

    public DbSet<CryptoPrice> CryptoPrices => Set<CryptoPrice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CryptoPrice>(entity =>
        {
            entity.ToTable("crypto_prices");
            entity.HasKey(c => c.id);
            entity.HasIndex(c => new { c.coingecko_id, c.currency }).IsUnique();
            entity.Property(c => c.coingecko_id).HasMaxLength(100).IsRequired();
            entity.Property(c => c.symbol).HasMaxLength(20);
            entity.Property(c => c.name).HasMaxLength(100);
            entity.Property(c => c.currency).HasMaxLength(10);
        });

        base.OnModelCreating(modelBuilder);
    }
}
