using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.Portfolios;
using CryptoAppBackEnd.Infraestructure.Persistence;
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
            return await _context.PortfolioAssets.AsNoTracking().ToListAsync();
        }

        public async Task<PortfolioAsset> GetPortfolioAssetAsync(int id)
        {
            return (await _context.PortfolioAssets.FindAsync(id))!;
        }

        public async Task CreatePortfolioAsset(PortfolioAsset portfolioAsset)
        {
            await _context.PortfolioAssets.AddAsync(portfolioAsset);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePortfolioAssetAsync(int id, PortfolioAsset portfolioAsset)
        {
            var existing = await _context.PortfolioAssets.FindAsync(id);
            if (existing is null)
            {
                return;
            }

            existing.UpdateHolding(
                portfolioAsset.portfolio_id,
                portfolioAsset.crypto_id,
                portfolioAsset.quantity,
                portfolioAsset.average_buy_price,
                portfolioAsset.total_invested);

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
