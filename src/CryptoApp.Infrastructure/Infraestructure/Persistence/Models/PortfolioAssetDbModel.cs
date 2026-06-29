namespace CryptoAppBackEnd.Infraestructure.Persistence.Models
{
    /// <summary>
    /// Modelo de persistencia (POCO) de la tabla "portfolio_assets".
    /// </summary>
    public class PortfolioAssetDbModel
    {
        public int id { get; set; }
        public int portfolio_id { get; set; }
        public int crypto_id { get; set; }
        public decimal quantity { get; set; }
        public decimal average_buy_price { get; set; }
        public decimal total_invested { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
