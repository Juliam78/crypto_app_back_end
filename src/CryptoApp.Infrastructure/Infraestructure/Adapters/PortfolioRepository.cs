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

            existing.UpdateDetails(portfolio.person_id, portfolio.name, portfolio.base_currency);

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
