namespace TradingService;

/// <summary>
/// Movimiento de compra/venta. Desnormalizado a propósito: guarda el nombre/símbolo de la
/// moneda y el id del usuario como string, sin claves foráneas a otros servicios.
/// type: 'B' = compra, 'S' = venta.
/// </summary>
public class Movement
{
    public int id { get; set; }
    public string user_id { get; set; } = string.Empty;
    public string user_name { get; set; } = string.Empty;
    public string coin_id { get; set; } = string.Empty;
    public string coin_name { get; set; } = string.Empty;
    public string coin_symbol { get; set; } = string.Empty;
    public char type { get; set; }
    public decimal quantity { get; set; }
    public string currency { get; set; } = "usd";
    public decimal price { get; set; }
    public decimal total { get; set; }
    public decimal realized_pnl { get; set; }
    public DateTime created_at { get; set; } = DateTime.UtcNow;
}

/// <summary>DTO con el shape exacto que espera el frontend (shared/types.ts -> Movement).</summary>
public record MovementDto(
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
    public static MovementDto From(Movement m) => new(
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

/// <summary>Solicitud de operación enviada por el frontend.</summary>
public record TradeRequest(
    string userId,
    string? userName,
    string coinId,
    string coinName,
    string coinSymbol,
    string type,        // 'buy' | 'sell'
    decimal amountUsd,
    decimal priceUsd,
    string? currency);
