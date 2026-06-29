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
            var jane = new Person("Jane Smith", "jane.smith@example.com", (char)TypePerson.User);
            jane.SetPasswordHash("hashed_password");
            jane.Activate();
            var carl = new Person("Carl Smith", "carl.smith@example.com", (char)TypePerson.Employee);
            carl.SetPasswordHash("hashed_password");
            carl.Activate();
            var john = new Person("John Doe", "john.doe@example.com", (char)TypePerson.Admin);
            john.SetPasswordHash("hashed_password");
            john.Activate();
            context.Persons.AddRange(john, jane, carl);

            // Criptomonedas
            var btc = new CryptoCurrency("BTC", "Bitcoin", 50000m, 2.35m, 980000000000m);
            btc.SetImageUrl("https://example.com/btc.png");
            var eth = new CryptoCurrency("ETH", "Ethereum", 4000m, 1.15m, 480000000000m);
            eth.SetImageUrl("https://example.com/eth.png");
            var ada = new CryptoCurrency("ADA", "Cardano", 2.5m, -0.45m, 85000000000m);
            ada.SetImageUrl("https://example.com/ada.png");
            context.CryptoCurrencies.AddRange(btc, eth, ada);

            context.SaveChanges();

            // Portafolios (dependen de las personas ya persistidas)
            var janePortfolio = new Portfolio("Jane - largo plazo", "USD");
            janePortfolio.AssignToPerson(jane.id);
            var carlPortfolio = new Portfolio("Carl - trading", "USD");
            carlPortfolio.AssignToPerson(carl.id);
            context.Portfolios.AddRange(janePortfolio, carlPortfolio);

            context.SaveChanges();

            // Activos del portafolio
            var janeBtc = new PortfolioAsset(0.35m, 48000m, 16800m);
            janeBtc.AssignTo(janePortfolio.id, btc.id);
            var janeEth = new PortfolioAsset(2.25m, 3900m, 8775m);
            janeEth.AssignTo(janePortfolio.id, eth.id);
            var carlAda = new PortfolioAsset(1500m, 2.1m, 3150m);
            carlAda.AssignTo(carlPortfolio.id, ada.id);
            context.PortfolioAssets.AddRange(janeBtc, janeEth, carlAda);

            // Movimientos
            var janeBtcMov = new Movement('B', 0.35m, 48000m, 16800m, 0m);
            janeBtcMov.AssignContext(jane.id, janePortfolio.id, btc.id);
            var janeEthMov = new Movement('B', 2.25m, 3900m, 8775m, 0m);
            janeEthMov.AssignContext(jane.id, janePortfolio.id, eth.id);
            var carlAdaMov = new Movement('B', 1500m, 2.1m, 3150m, 0m);
            carlAdaMov.AssignContext(carl.id, carlPortfolio.id, ada.id);
            context.Movements.AddRange(janeBtcMov, janeEthMov, carlAdaMov);

            context.SaveChanges();
        }
    }
}
