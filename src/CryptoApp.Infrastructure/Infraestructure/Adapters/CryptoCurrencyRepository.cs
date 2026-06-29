using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.CryptoCurrencies;
using CryptoAppBackEnd.Infraestructure.Persistence;
using CryptoAppBackEnd.Infraestructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace CryptoAppBackEnd.Infraestructure.Adapters
{
    public class CryptoCurrencyRepository : ICryptoCurrencyRepository
    {
        private readonly AppDbContext _context;

        public CryptoCurrencyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CryptoCurrency>> GetCryptoCurrenciesAsync()
        {
            var rows = await _context.CryptoCurrencies.AsNoTracking().ToListAsync();
            return rows.Select(CryptoCurrencyMapper.ToDomain).ToList();
        }

        public async Task<CryptoCurrency> GetCryptoCurrencyByIdAsync(int id)
        {
            var row = await _context.CryptoCurrencies.FindAsync(id);
            return row is null ? null! : CryptoCurrencyMapper.ToDomain(row);
        }

        public async Task CreateCryptoCurrency(CryptoCurrency cryptoCurrency)
        {
            await _context.CryptoCurrencies.AddAsync(CryptoCurrencyMapper.ToDb(cryptoCurrency));
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCryptoCurrency(int id, CryptoCurrency cryptoCurrency)
        {
            var existing = await _context.CryptoCurrencies.FindAsync(id);
            if (existing is null)
            {
                return;
            }

            existing.symbol = cryptoCurrency.symbol;
            existing.name = cryptoCurrency.name;
            existing.image_url = cryptoCurrency.image_url;
            existing.current_price = cryptoCurrency.current_price;
            existing.price_change_24h = cryptoCurrency.price_change_24h;
            existing.market_cap = cryptoCurrency.market_cap;
            existing.last_price_update = cryptoCurrency.last_price_update;
            existing.updated_at = cryptoCurrency.updated_at;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCryptoCurrency(int id)
        {
            var existing = await _context.CryptoCurrencies.FindAsync(id);
            if (existing is null)
            {
                return;
            }

            _context.CryptoCurrencies.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}
