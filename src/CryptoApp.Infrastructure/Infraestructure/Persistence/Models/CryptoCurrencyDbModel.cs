namespace CryptoAppBackEnd.Infraestructure.Persistence.Models
{
    /// <summary>
    /// Modelo de persistencia (POCO) de la tabla "crypto_currencies".
    /// </summary>
    public class CryptoCurrencyDbModel
    {
        public int id { get; set; }
        public string symbol { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string image_url { get; set; } = string.Empty;
        public decimal current_price { get; set; }
        public decimal price_change_24h { get; set; }
        public decimal market_cap { get; set; }
        public DateTime last_price_update { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
