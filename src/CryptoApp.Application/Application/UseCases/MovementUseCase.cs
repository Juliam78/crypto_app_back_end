using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.Movements;

namespace CryptoAppBackEnd.Application.UseCases
{
    /// <summary>Datos de una operación de trade enviada por el cliente.</summary>
    public record TradeCommand(
        string UserId,
        string? UserName,
        string CoinId,
        string CoinName,
        string CoinSymbol,
        string Type,        // "buy" | "sell"
        decimal AmountUsd,
        decimal PriceUsd,
        string? Currency);

    public class MovementUseCase
    {
        private readonly IMovementRepository _movementPort;

        public MovementUseCase(IMovementRepository movementPort)
        {
            _movementPort = movementPort;
        }

        /// <summary>
        /// Registra una compra/venta calculando la cantidad y el PnL realizado del lado servidor.
        /// Réplica del cálculo de TradingService: cantidad = monto/precio; en ventas
        /// realized_pnl = (precio_venta - costo_promedio_ponderado) * cantidad, con validación de
        /// saldo disponible sobre la posición previa del usuario+moneda+divisa.
        /// </summary>
        public async Task<Movement> RegisterTrade(TradeCommand req)
        {
            if (req.AmountUsd <= 0 || req.PriceUsd <= 0)
                throw new ArgumentException("Operación inválida: monto y precio deben ser mayores a 0.");

            var currency = string.IsNullOrWhiteSpace(req.Currency) ? "usd" : req.Currency;
            var isSell = string.Equals(req.Type, "sell", StringComparison.OrdinalIgnoreCase);
            var quantity = req.AmountUsd / req.PriceUsd;

            // Costo promedio actual de la posición (réplica de lib/portfolio.ts getAverageCost).
            var history = await _movementPort.GetPositionHistoryAsync(req.UserId, req.CoinId, currency);
            var (heldQuantity, averageCost) = ComputePosition(history);

            if (isSell && quantity > heldQuantity)
                throw new InvalidOperationException("No tienes saldo suficiente para vender esa cantidad.");

            var realizedPnl = isSell ? (req.PriceUsd - averageCost) * quantity : 0m;

            var movement = new Movement(
                req.UserId,
                req.UserName ?? string.Empty,
                req.CoinId,
                req.CoinName,
                req.CoinSymbol,
                isSell ? 'S' : 'B',
                quantity,
                currency,
                req.PriceUsd,
                req.AmountUsd,
                realizedPnl);

            await _movementPort.CreateMovementAsync(movement);
            return movement;
        }

        /// <summary>
        /// Historial de movimientos. role=user filtra por su userId; admin (u otro) ve todo.
        /// Ordenados por fecha descendente.
        /// </summary>
        public async Task<IEnumerable<Movement>> GetMovements(string? userId, string? role)
        {
            if (string.Equals(role, "user", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(userId))
            {
                return await _movementPort.GetByUserAsync(userId);
            }

            return await _movementPort.GetAllAsync();
        }

        // Cantidad retenida y costo promedio ponderado sobre movimientos ordenados por fecha asc.
        private static (decimal quantity, decimal averageCost) ComputePosition(IEnumerable<Movement> sorted)
        {
            decimal quantity = 0;
            decimal averageCost = 0;

            foreach (var m in sorted)
            {
                if (m.type == 'B')
                {
                    var nextQuantity = quantity + m.quantity;
                    var nextTotalCost = averageCost * quantity + m.price * m.quantity;
                    quantity = nextQuantity;
                    averageCost = nextQuantity > 0 ? nextTotalCost / nextQuantity : 0;
                }
                else
                {
                    quantity = Math.Max(0, quantity - m.quantity);
                    if (quantity == 0) averageCost = 0;
                }
            }

            return (quantity, averageCost);
        }
    }
}
