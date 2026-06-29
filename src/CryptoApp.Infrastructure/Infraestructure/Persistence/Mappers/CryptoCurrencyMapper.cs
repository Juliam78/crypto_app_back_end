using CryptoAppBackEnd.Domains.Entities.CryptoCurrencies;
using CryptoAppBackEnd.Infraestructure.Persistence.Models;

namespace CryptoAppBackEnd.Infraestructure.Persistence.Mappers
{
    public static class CryptoCurrencyMapper
    {
        public static CryptoCurrencyDbModel ToDb(CryptoCurrency domain) => new()
        {
            id = domain.id,
            symbol = domain.symbol,
            name = domain.name,
            image_url = domain.image_url,
            current_price = domain.current_price,
            price_change_24h = domain.price_change_24h,
            market_cap = domain.market_cap,
            last_price_update = domain.last_price_update,
            created_at = domain.created_at,
            updated_at = domain.updated_at
        };

        public static CryptoCurrency ToDomain(CryptoCurrencyDbModel db) => CryptoCurrency.FromPersistence(
            db.id,
            db.symbol,
            db.name,
            db.image_url,
            db.current_price,
            db.price_change_24h,
            db.market_cap,
            db.last_price_update,
            db.created_at,
            db.updated_at);
    }
}
