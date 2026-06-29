using CryptoAppBackEnd.Domains.Entities.Persons;
using CryptoAppBackEnd.Infraestructure.Persistence.Models;

namespace CryptoAppBackEnd.Infraestructure.Persistence
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Persons.Any())
            {
                return;
            }

            var now = DateTime.UtcNow;

            // Personas
            var jane = new PersonDbModel
            {
                name = "Jane Smith",
                email = "jane.smith@example.com",
                role = (char)TypePerson.User,
                password_hash = "hashed_password",
                status = true,
                created_at = now,
                updated_at = now
            };
            var carl = new PersonDbModel
            {
                name = "Carl Smith",
                email = "carl.smith@example.com",
                role = (char)TypePerson.Employee,
                password_hash = "hashed_password",
                status = true,
                created_at = now,
                updated_at = now
            };
            var john = new PersonDbModel
            {
                name = "John Doe",
                email = "john.doe@example.com",
                role = (char)TypePerson.Admin,
                password_hash = "hashed_password",
                status = true,
                created_at = now,
                updated_at = now
            };
            context.Persons.AddRange(john, jane, carl);

            // Criptomonedas
            var btc = new CryptoCurrencyDbModel
            {
                symbol = "BTC",
                name = "Bitcoin",
                image_url = "https://example.com/btc.png",
                current_price = 50000m,
                price_change_24h = 2.35m,
                market_cap = 980000000000m,
                last_price_update = now,
                created_at = now,
                updated_at = now
            };
            var eth = new CryptoCurrencyDbModel
            {
                symbol = "ETH",
                name = "Ethereum",
                image_url = "https://example.com/eth.png",
                current_price = 4000m,
                price_change_24h = 1.15m,
                market_cap = 480000000000m,
                last_price_update = now,
                created_at = now,
                updated_at = now
            };
            var ada = new CryptoCurrencyDbModel
            {
                symbol = "ADA",
                name = "Cardano",
                image_url = "https://example.com/ada.png",
                current_price = 2.5m,
                price_change_24h = -0.45m,
                market_cap = 85000000000m,
                last_price_update = now,
                created_at = now,
                updated_at = now
            };
            context.CryptoCurrencies.AddRange(btc, eth, ada);

            // Persiste personas y criptomonedas para obtener sus ids generados.
            context.SaveChanges();

            // Portafolios (dependen de las personas ya persistidas)
            var janePortfolio = new PortfolioDbModel
            {
                person_id = jane.id,
                name = "Jane - largo plazo",
                base_currency = "USD",
                created_at = now,
                updated_at = now
            };
            var carlPortfolio = new PortfolioDbModel
            {
                person_id = carl.id,
                name = "Carl - trading",
                base_currency = "USD",
                created_at = now,
                updated_at = now
            };
            context.Portfolios.AddRange(janePortfolio, carlPortfolio);

            context.SaveChanges();

            // Activos del portafolio
            var janeBtc = new PortfolioAssetDbModel
            {
                portfolio_id = janePortfolio.id,
                crypto_id = btc.id,
                quantity = 0.35m,
                average_buy_price = 48000m,
                total_invested = 16800m,
                created_at = now,
                updated_at = now
            };
            var janeEth = new PortfolioAssetDbModel
            {
                portfolio_id = janePortfolio.id,
                crypto_id = eth.id,
                quantity = 2.25m,
                average_buy_price = 3900m,
                total_invested = 8775m,
                created_at = now,
                updated_at = now
            };
            var carlAda = new PortfolioAssetDbModel
            {
                portfolio_id = carlPortfolio.id,
                crypto_id = ada.id,
                quantity = 1500m,
                average_buy_price = 2.1m,
                total_invested = 3150m,
                created_at = now,
                updated_at = now
            };
            context.PortfolioAssets.AddRange(janeBtc, janeEth, carlAda);

            // Movimientos (desnormalizados: sin FKs; guardan ids/nombres como datos planos).
            var janeBtcMov = new MovementDbModel
            {
                user_id = jane.id.ToString(),
                user_name = jane.name,
                coin_id = "bitcoin",
                coin_name = btc.name,
                coin_symbol = btc.symbol,
                type = 'B',
                quantity = 0.35m,
                currency = "usd",
                price = 48000m,
                total = 16800m,
                realized_pnl = 0m,
                created_at = now
            };
            var janeEthMov = new MovementDbModel
            {
                user_id = jane.id.ToString(),
                user_name = jane.name,
                coin_id = "ethereum",
                coin_name = eth.name,
                coin_symbol = eth.symbol,
                type = 'B',
                quantity = 2.25m,
                currency = "usd",
                price = 3900m,
                total = 8775m,
                realized_pnl = 0m,
                created_at = now
            };
            var carlAdaMov = new MovementDbModel
            {
                user_id = carl.id.ToString(),
                user_name = carl.name,
                coin_id = "cardano",
                coin_name = ada.name,
                coin_symbol = ada.symbol,
                type = 'B',
                quantity = 1500m,
                currency = "usd",
                price = 2.1m,
                total = 3150m,
                realized_pnl = 0m,
                created_at = now
            };
            context.Movements.AddRange(janeBtcMov, janeEthMov, carlAdaMov);

            context.SaveChanges();
        }
    }
}
