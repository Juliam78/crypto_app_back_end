using CryptoAppBackEnd.Domains.Shared;

namespace CryptoAppBackEnd.Domains.Entities.Portfolios
{
    public class PortfolioAsset
    {
        public int id { get; set; }
        public int portfolio_id { get; set; }
        public int crypto_id { get; set; }
        public decimal quantity { get; set; }
        public decimal average_buy_price { get; set; }
        public decimal total_invested { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        public DateTime updated_at { get; set; } = DateTime.UtcNow;

        // Parameterless constructor required by EF Core for materialization.
        public PortfolioAsset() { }

        public PortfolioAsset(decimal quantity, decimal average_buy_price, decimal total_invested)
        {
            Helpers.ValidateFields(
                (nameof(quantity), quantity),
                 (nameof(average_buy_price), average_buy_price),
                 (nameof(total_invested), total_invested)
            );

            this.quantity = quantity;
            this.average_buy_price = average_buy_price;
            this.total_invested = total_invested;
        }
    }
}
