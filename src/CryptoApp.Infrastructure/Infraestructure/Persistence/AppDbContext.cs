using CryptoAppBackEnd.Infraestructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptoAppBackEnd.Infraestructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<PersonDbModel> Persons => Set<PersonDbModel>();
        public DbSet<CryptoCurrencyDbModel> CryptoCurrencies => Set<CryptoCurrencyDbModel>();
        public DbSet<PortfolioDbModel> Portfolios => Set<PortfolioDbModel>();
        public DbSet<PortfolioAssetDbModel> PortfolioAssets => Set<PortfolioAssetDbModel>();
        public DbSet<MovementDbModel> Movements => Set<MovementDbModel>();
        public DbSet<CryptoPriceDbModel> CryptoPrices => Set<CryptoPriceDbModel>();
        public DbSet<AppErrorDbModel> AppErrors => Set<AppErrorDbModel>();
        public DbSet<LessonDbModel> Lessons => Set<LessonDbModel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
