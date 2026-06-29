using CryptoAppBackEnd.Domains.Entities.CryptoCurrencies;
using CryptoAppBackEnd.Domains.Entities.Movements;
using CryptoAppBackEnd.Domains.Entities.Persons;
using CryptoAppBackEnd.Domains.Entities.Portfolios;
using Microsoft.EntityFrameworkCore;

namespace CryptoAppBackEnd.Infraestructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Person> Persons => Set<Person>();
        public DbSet<CryptoCurrency> CryptoCurrencies => Set<CryptoCurrency>();
        public DbSet<Portfolio> Portfolios => Set<Portfolio>();
        public DbSet<PortfolioAsset> PortfolioAssets => Set<PortfolioAsset>();
        public DbSet<Movement> Movements => Set<Movement>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
