using CryptoAppBackEnd.Domains.Shared;

namespace CryptoAppBackEnd.Domains.Entities.Movements
{
    public class Movement
    {
        public int id { get; private set; }
        public int person_id { get; private set; }
        public int portfolio_id { get; private set; }
        public int crypto_id { get; private set; }
        public char type { get; private set; }
        public decimal quantity { get; private set; }
        public decimal price { get; private set; }
        public decimal total { get; private set; }
        public decimal realized_pnl { get; private set; }
        public DateTime created_at { get; private set; } = DateTime.UtcNow;

        // Parameterless constructor required by EF Core for materialization.
        private Movement() { }

        public Movement(char type, decimal quantity, decimal price, decimal total, decimal realized_pnl)
        {
            // realized_pnl se excluye a propósito: una pérdida realizada (negativa) es válida.
            Helpers.ValidateFields(
                (nameof(type), type),
                (nameof(quantity), quantity),
                (nameof(price), price),
                (nameof(total), total)
            );

            this.type = type;
            this.quantity = quantity;
            this.price = price;
            this.total = total;
            this.realized_pnl = realized_pnl;
            this.created_at = DateTime.UtcNow;
        }

        public void AssignContext(int personId, int portfolioId, int cryptoId)
        {
            Helpers.ValidateFields(
                (nameof(personId), personId),
                (nameof(portfolioId), portfolioId),
                (nameof(cryptoId), cryptoId)
            );
            this.person_id = personId;
            this.portfolio_id = portfolioId;
            this.crypto_id = cryptoId;
        }

        public void MarkCreated()
        {
            this.created_at = DateTime.UtcNow;
        }
    }
}
