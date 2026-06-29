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

            existing.UpdateDetails(
                cryptoCurrency.symbol,
                cryptoCurrency.name,
                cryptoCurrency.image_url,
                cryptoCurrency.current_price,
                cryptoCurrency.price_change_24h,
                cryptoCurrency.market_cap);

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
