using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.CryptoCurrencies;
using CryptoAppBackEnd.Infraestructure.Persistence;
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
            return await _context.CryptoCurrencies.AsNoTracking().ToListAsync();
        }

        public async Task<CryptoCurrency> GetCryptoCurrencyByIdAsync(int id)
        {
            return (await _context.CryptoCurrencies.FindAsync(id))!;
        }

        public async Task CreateCryptoCurrency(CryptoCurrency cryptoCurrency)
        {
            cryptoCurrency.created_at = DateTime.UtcNow;
            cryptoCurrency.updated_at = DateTime.UtcNow;
            cryptoCurrency.last_price_update = DateTime.UtcNow;
            await _context.CryptoCurrencies.AddAsync(cryptoCurrency);
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
            existing.last_price_update = DateTime.UtcNow;
            existing.updated_at = DateTime.UtcNow;

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
