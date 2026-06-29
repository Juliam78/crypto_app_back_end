using CryptoAppBackEnd.Domains.Entities.Movements;

namespace CryptoApp.Web.Contracts
{
    /// <summary>
    /// Solicitud de operación enviada por el frontend (shape idéntico al microservicio).
    /// type: "buy" | "sell".
    /// </summary>
    public record TradeRequest(
        string userId,
        string? userName,
        string coinId,
        string coinName,
        string coinSymbol,
        string type,
        decimal amountUsd,
        decimal priceUsd,
        string? currency);

    /// <summary>
    /// DTO con el shape exacto que espera el frontend (shared/types.ts -> Movement).
    /// id se serializa como string; type como "buy"/"sell"; created_at en ISO 8601.
    /// </summary>
    public record MovementResponse(
        string id,
        string user_id,
        string user_name,
        string coin_id,
        string coin_name,
        string coin_symbol,
        string type,
        decimal quantity,
        string currency,
        decimal price,
        decimal total,
        decimal realized_pnl,
        string created_at)
    {
        public static MovementResponse From(Movement m) => new(
            m.id.ToString(),
            m.user_id,
            m.user_name,
            m.coin_id,
            m.coin_name,
            m.coin_symbol,
            m.type == 'S' ? "sell" : "buy",
            m.quantity,
            m.currency,
            m.price,
            m.total,
            m.realized_pnl,
            m.created_at.ToString("o"));
    }
}
