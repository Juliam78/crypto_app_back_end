using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.Portfolios;
using CryptoAppBackEnd.Infraestructure.Persistence;
using CryptoAppBackEnd.Infraestructure.Persistence.Mappers;
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
            var rows = await _context.Portfolios.AsNoTracking().ToListAsync();
            return rows.Select(PortfolioMapper.ToDomain).ToList();
        }

        public async Task<Portfolio> GetPortfolioAsync(int id)
        {
            var row = await _context.Portfolios.FindAsync(id);
            return row is null ? null! : PortfolioMapper.ToDomain(row);
        }

        public async Task CreatePortfolioAsync(Portfolio portfolio)
        {
            await _context.Portfolios.AddAsync(PortfolioMapper.ToDb(portfolio));
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
            existing.updated_at = portfolio.updated_at;

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
