using CryptoAppBackEnd.Domains.Shared;

namespace CryptoAppBackEnd.Domains.Entities.CryptoCurrencies
{
    public class CryptoCurrency
    {
        public int id { get; set; }
        public string symbol { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string image_url { get; set; } = string.Empty;
        public decimal current_price { get; set; }
        public decimal price_change_24h { get; set; }
        public decimal market_cap { get; set; }
        public DateTime last_price_update { get; set; } = DateTime.UtcNow;
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        public DateTime updated_at { get; set; } = DateTime.UtcNow;

        // Parameterless constructor required by EF Core for materialization.
        public CryptoCurrency() { }

        public CryptoCurrency(string symbol, string name, decimal current_price, decimal price_change_24h, decimal market_cap)
        {
            Helpers.ValidateFields(
                (nameof(symbol), symbol),
                (nameof(name), name),
                (nameof(current_price), current_price),
                (nameof(price_change_24h), price_change_24h),
                (nameof(market_cap), market_cap)
            );

            this.symbol = symbol;
            this.name = name;
            this.current_price = current_price;
            this.price_change_24h = price_change_24h;
            this.market_cap = market_cap;
        }
    }
}
