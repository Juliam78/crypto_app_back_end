using System.Text.Json.Serialization;

namespace MarketService;

/// <summary>
/// Coin con exactamente las mismas claves que espera el frontend (shared/types.ts -> Coin)
/// y que devuelve CoinGecko. Sirve tanto para deserializar la API como para responder al front.
/// </summary>
public class Coin
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("symbol")] public string Symbol { get; set; } = string.Empty;
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("image")] public string Image { get; set; } = string.Empty;
    // CoinGecko puede devolver null en campos numéricos para algunas monedas;
    // se modelan como nullable y se normalizan a 0 antes de responder/cachear.
    [JsonPropertyName("current_price")] public decimal? CurrentPrice { get; set; }
    [JsonPropertyName("market_cap")] public decimal? MarketCap { get; set; }
    [JsonPropertyName("market_cap_rank")] public int? MarketCapRank { get; set; }
    [JsonPropertyName("total_volume")] public decimal? TotalVolume { get; set; }
    [JsonPropertyName("high_24h")] public decimal? High24h { get; set; }
    [JsonPropertyName("low_24h")] public decimal? Low24h { get; set; }
    [JsonPropertyName("price_change_percentage_24h")] public decimal? PriceChangePercentage24h { get; set; }
    [JsonPropertyName("sparkline_in_7d")] public Sparkline? SparklineIn7d { get; set; }

    // Sustituye nulos por 0 para que el frontend reciba siempre números.
    public void Normalize()
    {
        CurrentPrice ??= 0m;
        MarketCap ??= 0m;
        MarketCapRank ??= 0;
        TotalVolume ??= 0m;
        High24h ??= 0m;
        Low24h ??= 0m;
        PriceChangePercentage24h ??= 0m;
        SparklineIn7d ??= new Sparkline();
    }
}

public class Sparkline
{
    [JsonPropertyName("price")] public List<decimal> Price { get; set; } = new();
}

/// <summary>
/// Snapshot persistido del precio (caché en la Postgres del servicio de mercado).
/// </summary>
public class CryptoPrice
{
    public int id { get; set; }
    public string coingecko_id { get; set; } = string.Empty;
    public string symbol { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string image_url { get; set; } = string.Empty;
    public string currency { get; set; } = "usd";
    public decimal current_price { get; set; }
    public decimal price_change_24h { get; set; }
    public decimal market_cap { get; set; }
    // JSON completo de la moneda, para servir desde caché con plena fidelidad si CoinGecko falla.
    public string payload { get; set; } = string.Empty;
    public DateTime last_updated { get; set; } = DateTime.UtcNow;
}
