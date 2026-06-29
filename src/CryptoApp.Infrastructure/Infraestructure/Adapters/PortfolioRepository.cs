using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.Portfolios;
using CryptoAppBackEnd.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CryptoAppBackEnd.Infraestructure.Adapters
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly AppDbContext _context;

        public PortfolioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Portfolio>> GetPortfoliosAsync()
        {
            return await _context.Portfolios.AsNoTracking().ToListAsync();
        }

        public async Task<Portfolio> GetPortfolioAsync(int id)
        {
            return (await _context.Portfolios.FindAsync(id))!;
        }

        public async Task CreatePortfolioAsync(Portfolio portfolio)
        {
            portfolio.created_at = DateTime.UtcNow;
            portfolio.updated_at = DateTime.UtcNow;
            await _context.Portfolios.AddAsync(portfolio);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePortfolioAsync(int id, Portfolio portfolio)
        {
            var existing = await _context.Portfolios.FindAsync(id);
            if (existing is null)
            {
                return;
            }

            existing.person_id = portfolio.person_id;
            existing.name = portfolio.name;
            existing.base_currency = portfolio.base_currency;
            existing.updated_at = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeletePortfolioAsync(int id)
        {
            var existing = await _context.Portfolios.FindAsync(id);
            if (existing is null)
            {
                return;
            }

            _context.Portfolios.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}
