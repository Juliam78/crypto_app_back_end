using System.Text.Json;
using CryptoAppBackEnd.Application.Market;
using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Infraestructure.Persistence;
using CryptoAppBackEnd.Infraestructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptoAppBackEnd.Infraestructure.Market
{
    /// <summary>
    /// Adaptador de caché de mercado en BD (implementa <see cref="IMarketCache"/>).
    /// Guarda un único snapshot por divisa con la lista completa de monedas serializada en JSON,
    /// para servir desde caché con plena fidelidad si CoinGecko no está disponible.
    /// </summary>
    public class MarketCacheRepository : IMarketCache
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        private readonly AppDbContext _context;

        public MarketCacheRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(string currency, IReadOnlyList<MarketCoin> coins)
        {
            var payload = JsonSerializer.Serialize(coins, JsonOptions);

            var existing = await _context.CryptoPrices
                .FirstOrDefaultAsync(c => c.currency == currency);

            if (existing is null)
            {
                _context.CryptoPrices.Add(new CryptoPriceDbModel
                {
                    currency = currency,
                    payload = payload,
                    last_updated = DateTime.UtcNow,
                });
            }
            else
            {
                existing.payload = payload;
                existing.last_updated = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<MarketCoin>> LoadAsync(string currency)
        {
            var row = await _context.CryptoPrices.AsNoTracking()
                .FirstOrDefaultAsync(c => c.currency == currency && c.payload != "");

            if (row is null)
            {
                return new List<MarketCoin>();
            }

            var coins = JsonSerializer.Deserialize<List<MarketCoin>>(row.payload, JsonOptions);
            return coins ?? new List<MarketCoin>();
        }
    }
}
