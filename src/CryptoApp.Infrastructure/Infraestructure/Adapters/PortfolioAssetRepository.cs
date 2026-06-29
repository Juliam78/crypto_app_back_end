using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.Portfolios;
using CryptoAppBackEnd.Infraestructure.Persistence;
using CryptoAppBackEnd.Infraestructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace CryptoAppBackEnd.Infraestructure.Adapters
{
    public class PortfolioAssetRepository : IPortfolioAssetRepository
    {
        private readonly AppDbContext _context;

        public PortfolioAssetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PortfolioAsset>> GetPortfolioAssetsAsync()
        {
            var rows = await _context.PortfolioAssets.AsNoTracking().ToListAsync();
            return rows.Select(PortfolioAssetMapper.ToDomain).ToList();
        }

        public async Task<PortfolioAsset> GetPortfolioAssetAsync(int id)
        {
            var row = await _context.PortfolioAssets.FindAsync(id);
            return row is null ? null! : PortfolioAssetMapper.ToDomain(row);
        }

        public async Task CreatePortfolioAsset(PortfolioAsset portfolioAsset)
        {
            await _context.PortfolioAssets.AddAsync(PortfolioAssetMapper.ToDb(portfolioAsset));
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePortfolioAssetAsync(int id, PortfolioAsset portfolioAsset)
        {
            var existing = await _context.PortfolioAssets.FindAsync(id);
            if (existing is null)
            {
                return;
            }

            existing.portfolio_id = portfolioAsset.portfolio_id;
            existing.crypto_id = portfolioAsset.crypto_id;
            existing.quantity = portfolioAsset.quantity;
            existing.average_buy_price = portfolioAsset.average_buy_price;
            existing.total_invested = portfolioAsset.total_invested;
            existing.updated_at = portfolioAsset.updated_at;

            await _context.SaveChangesAsync();
        }

        public async Task DeletePortfolioAssetAsync(int id)
        {
            var existing = await _context.PortfolioAssets.FindAsync(id);
            if (existing is null)
            {
                return;
            }

            _context.PortfolioAssets.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}
