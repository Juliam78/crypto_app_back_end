using CryptoAppBackEnd.Domains.Shared;

namespace CryptoAppBackEnd.Domains.Entities.Movements
{
    public class Movement
    {
        public int id { get; set; }
        public int person_id { get; set; }
        public int portfolio_id { get; set; }
        public int crypto_id { get; set; }
        public char type { get; set; }
        public decimal quantity { get; set; }
        public decimal price { get; set; }
        public decimal total { get; set; }
        public decimal realized_pnl { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // Parameterless constructor required by EF Core for materialization.
        public Movement() { }

        public Movement(char type, decimal quantity, decimal price, decimal total, decimal realized_pnl)
        {
            Helpers.ValidateFields(
                (nameof(type), type),
                (nameof(quantity), quantity),
                (nameof(price), price),
                (nameof(total), total),
                (nameof(realized_pnl), realized_pnl)
            );

            this.type = type;
            this.quantity = quantity;
            this.price = price;
            this.total = total;
            this.realized_pnl = realized_pnl;
        }
    }
}
