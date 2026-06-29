using CryptoAppBackEnd.Domains.Shared;

namespace CryptoAppBackEnd.Domains.Entities.Movements
{
    /// <summary>
    /// Movimiento de compra/venta. Desnormalizado a propósito (igual que el microservicio
    /// TradingService): guarda el nombre/símbolo de la moneda y el id/nombre del usuario como
    /// string, sin claves foráneas a otras tablas. type: 'B' = compra, 'S' = venta.
    /// </summary>
    public class Movement
    {
        public int id { get; private set; }
        public string user_id { get; private set; } = string.Empty;
        public string user_name { get; private set; } = string.Empty;
        public string coin_id { get; private set; } = string.Empty;
        public string coin_name { get; private set; } = string.Empty;
        public string coin_symbol { get; private set; } = string.Empty;
        public char type { get; private set; }
        public decimal quantity { get; private set; }
        public string currency { get; private set; } = "usd";
        public decimal price { get; private set; }
        public decimal total { get; private set; }
        public decimal realized_pnl { get; private set; }
        public DateTime created_at { get; private set; } = DateTime.UtcNow;

        // Parameterless constructor required by EF Core / design-time materialization.
        private Movement() { }

        /// <summary>
        /// Crea un movimiento nuevo, validado. realized_pnl se excluye a propósito de la
        /// validación de no-negatividad: una pérdida realizada (negativa) es válida.
        /// </summary>
        public Movement(
            string user_id,
            string user_name,
            string coin_id,
            string coin_name,
            string coin_symbol,
            char type,
            decimal quantity,
            string currency,
            decimal price,
            decimal total,
            decimal realized_pnl)
        {
            Helpers.ValidateFields(
                (nameof(user_id), user_id),
                (nameof(coin_id), coin_id),
                (nameof(coin_name), coin_name),
                (nameof(coin_symbol), coin_symbol),
                (nameof(type), type),
                (nameof(quantity), quantity),
                (nameof(currency), currency),
                (nameof(price), price),
                (nameof(total), total)
            );

            this.user_id = user_id;
            this.user_name = user_name ?? string.Empty;
            this.coin_id = coin_id;
            this.coin_name = coin_name;
            this.coin_symbol = coin_symbol;
            this.type = type;
            this.quantity = quantity;
            this.currency = currency;
            this.price = price;
            this.total = total;
            this.realized_pnl = realized_pnl;
            this.created_at = DateTime.UtcNow;
        }

        /// <summary>
        /// Rehidrata un Movement desde almacenamiento (estado completo, sin validación).
        /// </summary>
        public static Movement FromPersistence(
            int id,
            string user_id,
            string user_name,
            string coin_id,
            string coin_name,
            string coin_symbol,
            char type,
            decimal quantity,
            string currency,
            decimal price,
            decimal total,
            decimal realized_pnl,
            DateTime created_at)
        {
            return new Movement
            {
                id = id,
                user_id = user_id,
                user_name = user_name,
                coin_id = coin_id,
                coin_name = coin_name,
                coin_symbol = coin_symbol,
                type = type,
                quantity = quantity,
                currency = currency,
                price = price,
                total = total,
                realized_pnl = realized_pnl,
                created_at = created_at
            };
        }
    }
}
