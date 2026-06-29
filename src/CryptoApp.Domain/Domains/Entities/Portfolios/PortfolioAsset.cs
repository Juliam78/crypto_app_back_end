using CryptoAppBackEnd.Domains.Shared;

namespace CryptoAppBackEnd.Domains.Entities.Portfolios
{
    public class PortfolioAsset
    {
        public int id { get; private set; }
        public int portfolio_id { get; private set; }
        public int crypto_id { get; private set; }
        public decimal quantity { get; private set; }
        public decimal average_buy_price { get; private set; }
        public decimal total_invested { get; private set; }
        public DateTime created_at { get; private set; } = DateTime.UtcNow;
        public DateTime updated_at { get; private set; } = DateTime.UtcNow;

        // Parameterless constructor required by EF Core for materialization.
        private PortfolioAsset() { }

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
            this.created_at = DateTime.UtcNow;
            this.updated_at = DateTime.UtcNow;
        }

        /// <summary>
        /// Rehidrata un PortfolioAsset desde almacenamiento (estado completo, sin validación).
        /// </summary>
        public static PortfolioAsset FromPersistence(
            int id,
            int portfolio_id,
            int crypto_id,
            decimal quantity,
            decimal average_buy_price,
            decimal total_invested,
            DateTime created_at,
            DateTime updated_at)
        {
            return new PortfolioAsset
            {
                id = id,
                portfolio_id = portfolio_id,
                crypto_id = crypto_id,
                quantity = quantity,
                average_buy_price = average_buy_price,
                total_invested = total_invested,
                created_at = created_at,
                updated_at = updated_at
            };
        }

        public void AssignTo(int portfolioId, int cryptoId)
        {
            Helpers.ValidateFields(
                (nameof(portfolioId), portfolioId),
                (nameof(cryptoId), cryptoId)
            );
            this.portfolio_id = portfolioId;
            this.crypto_id = cryptoId;
            Touch();
        }

        public void UpdateHolding(int portfolio_id, int crypto_id, decimal quantity, decimal average_buy_price, decimal total_invested)
        {
            Helpers.ValidateFields(
                (nameof(portfolio_id), portfolio_id),
                (nameof(crypto_id), crypto_id),
                (nameof(quantity), quantity),
                (nameof(average_buy_price), average_buy_price),
                (nameof(total_invested), total_invested)
            );

            this.portfolio_id = portfolio_id;
            this.crypto_id = crypto_id;
            this.quantity = quantity;
            this.average_buy_price = average_buy_price;
            this.total_invested = total_invested;
            Touch();
        }

        private void Touch()
        {
            this.updated_at = DateTime.UtcNow;
        }
    }
}
