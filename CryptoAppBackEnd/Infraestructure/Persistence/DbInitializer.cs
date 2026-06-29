using CryptoAppBackEnd.Domains.Entities.CryptoCurrencies;
using CryptoAppBackEnd.Domains.Entities.Movements;
using CryptoAppBackEnd.Domains.Entities.Persons;
using CryptoAppBackEnd.Domains.Entities.Portfolios;

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

            // Personas
            var jane = new Person("Jane Smith", "jane.smith@example.com", (char)TypePerson.User) { password_hash = "hashed_password", status = true };
            var carl = new Person("Carl Smith", "carl.smith@example.com", (char)TypePerson.Employee) { password_hash = "hashed_password", status = true };
            var john = new Person("John Doe", "john.doe@example.com", (char)TypePerson.Admin) { password_hash = "hashed_password", status = true };
            context.Persons.AddRange(john, jane, carl);

            // Criptomonedas
            var btc = new CryptoCurrency("BTC", "Bitcoin", 50000m, 2.35m, 980000000000m) { image_url = "https://example.com/btc.png" };
            var eth = new CryptoCurrency("ETH", "Ethereum", 4000m, 1.15m, 480000000000m) { image_url = "https://example.com/eth.png" };
            var ada = new CryptoCurrency("ADA", "Cardano", 2.5m, -0.45m, 85000000000m) { image_url = "https://example.com/ada.png" };
            context.CryptoCurrencies.AddRange(btc, eth, ada);

            context.SaveChanges();

            // Portafolios (dependen de las personas ya persistidas)
            var janePortfolio = new Portfolio("Jane - largo plazo", "USD") { person_id = jane.id };
            var carlPortfolio = new Portfolio("Carl - trading", "USD") { person_id = carl.id };
            context.Portfolios.AddRange(janePortfolio, carlPortfolio);

            context.SaveChanges();

            // Activos del portafolio
            context.PortfolioAssets.AddRange(
                new PortfolioAsset(0.35m, 48000m, 16800m) { portfolio_id = janePortfolio.id, crypto_id = btc.id },
                new PortfolioAsset(2.25m, 3900m, 8775m) { portfolio_id = janePortfolio.id, crypto_id = eth.id },
                new PortfolioAsset(1500m, 2.1m, 3150m) { portfolio_id = carlPortfolio.id, crypto_id = ada.id }
            );

            // Movimientos
            context.Movements.AddRange(
                new Movement('B', 0.35m, 48000m, 16800m, 0m) { person_id = jane.id, portfolio_id = janePortfolio.id, crypto_id = btc.id },
                new Movement('B', 2.25m, 3900m, 8775m, 0m) { person_id = jane.id, portfolio_id = janePortfolio.id, crypto_id = eth.id },
                new Movement('B', 1500m, 2.1m, 3150m, 0m) { person_id = carl.id, portfolio_id = carlPortfolio.id, crypto_id = ada.id }
            );

            context.SaveChanges();
        }
    }
}
