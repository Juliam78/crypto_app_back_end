using CryptoAppBackEnd.Domains.Shared;

namespace CryptoAppBackEnd.Domains.Entities.CryptoCurrencies
{
    public class CryptoCurrency
    {
        public int id { get; private set; }
        public string symbol { get; private set; } = string.Empty;
        public string name { get; private set; } = string.Empty;
        public string image_url { get; private set; } = string.Empty;
        public decimal current_price { get; private set; }
        public decimal price_change_24h { get; private set; }
        public decimal market_cap { get; private set; }
        public DateTime last_price_update { get; private set; } = DateTime.UtcNow;
        public DateTime created_at { get; private set; } = DateTime.UtcNow;
        public DateTime updated_at { get; private set; } = DateTime.UtcNow;

        // Parameterless constructor required by EF Core for materialization.
        private CryptoCurrency() { }

        public CryptoCurrency(string symbol, string name, decimal current_price, decimal price_change_24h, decimal market_cap)
        {
            // price_change_24h se excluye a propósito: una variación negativa es válida de negocio.
            Helpers.ValidateFields(
                (nameof(symbol), symbol),
                (nameof(name), name),
                (nameof(current_price), current_price),
                (nameof(market_cap), market_cap)
            );

            this.symbol = symbol;
            this.name = name;
            this.current_price = current_price;
            this.price_change_24h = price_change_24h;
            this.market_cap = market_cap;
            var now = DateTime.UtcNow;
            this.last_price_update = now;
            this.created_at = now;
            this.updated_at = now;
        }

        /// <summary>
        /// Rehidrata una CryptoCurrency desde almacenamiento (estado completo, sin validación).
        /// </summary>
        public static CryptoCurrency FromPersistence(
            int id,
            string symbol,
            string name,
            string image_url,
            decimal current_price,
            decimal price_change_24h,
            decimal market_cap,
            DateTime last_price_update,
            DateTime created_at,
            DateTime updated_at)
        {
            return new CryptoCurrency
            {
                id = id,
                symbol = symbol,
                name = name,
                image_url = image_url,
                current_price = current_price,
                price_change_24h = price_change_24h,
                market_cap = market_cap,
                last_price_update = last_price_update,
                created_at = created_at,
                updated_at = updated_at
            };
        }

        public void SetImageUrl(string imageUrl)
        {
            this.image_url = imageUrl;
            Touch();
        }

        public void UpdateDetails(string symbol, string name, string image_url, decimal current_price, decimal price_change_24h, decimal market_cap)
        {
            // price_change_24h se excluye a propósito: una variación negativa es válida de negocio.
            Helpers.ValidateFields(
                (nameof(symbol), symbol),
                (nameof(name), name),
                (nameof(current_price), current_price),
                (nameof(market_cap), market_cap)
            );

            this.symbol = symbol;
            this.name = name;
            this.image_url = image_url;
            this.current_price = current_price;
            this.price_change_24h = price_change_24h;
            this.market_cap = market_cap;
            this.last_price_update = DateTime.UtcNow;
            Touch();
        }

        private void Touch()
        {
            this.updated_at = DateTime.UtcNow;
        }
    }
}
