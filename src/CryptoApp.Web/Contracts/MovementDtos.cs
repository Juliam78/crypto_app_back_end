using CryptoAppBackEnd.Domains.Entities.Movements;

namespace CryptoApp.Web.Contracts
{
    /// <summary>
    /// Mapeo de tipo de movimiento entre el dominio (char) y el contrato del frontend (string).
    /// 'B' &lt;-&gt; "buy"; 'S' &lt;-&gt; "sell".
    /// </summary>
    public static class MovementTypeMapping
    {
        public const char BuyChar = 'B';
        public const char SellChar = 'S';

        public static string ToContract(char type) => type == SellChar ? "sell" : "buy";

        public static char ToDomain(string? type) =>
            string.Equals(type, "sell", StringComparison.OrdinalIgnoreCase) ? SellChar : BuyChar;
    }

    public record CreateMovementRequest(
        int PersonId,
        int PortfolioId,
        int CryptoId,
        string Type,
        decimal Quantity,
        decimal Price,
        decimal Total,
        decimal RealizedPnl);

    public record MovementResponse(
        string Id,
        string PersonId,
        string PortfolioId,
        string CryptoId,
        string Type,
        decimal Quantity,
        decimal Price,
        decimal Total,
        decimal RealizedPnl,
        string CreatedAt)
    {
        public static MovementResponse From(Movement m) => new(
            m.id.ToString(),
            m.person_id.ToString(),
            m.portfolio_id.ToString(),
            m.crypto_id.ToString(),
            MovementTypeMapping.ToContract(m.type),
            m.quantity,
            m.price,
            m.total,
            m.realized_pnl,
            m.created_at.ToString("o"));
    }
}
