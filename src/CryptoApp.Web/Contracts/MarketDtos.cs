using System.Text.Json.Serialization;
using CryptoAppBackEnd.Application.Market;

namespace CryptoApp.Web.Contracts
{
    /// <summary>
    /// Respuesta de mercado con las claves EXACTAS que espera el frontend
    /// (mismas que devuelve CoinGecko: snake_case y sparkline_in_7d anidado).
    /// </summary>
    public record MarketCoinResponse(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("symbol")] string Symbol,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("image")] string Image,
        [property: JsonPropertyName("current_price")] decimal CurrentPrice,
        [property: JsonPropertyName("market_cap")] decimal MarketCap,
        [property: JsonPropertyName("market_cap_rank")] int MarketCapRank,
        [property: JsonPropertyName("total_volume")] decimal TotalVolume,
        [property: JsonPropertyName("high_24h")] decimal High24h,
        [property: JsonPropertyName("low_24h")] decimal Low24h,
        [property: JsonPropertyName("price_change_percentage_24h")] decimal PriceChangePercentage24h,
        [property: JsonPropertyName("sparkline_in_7d")] SparklineResponse Sparkline7d)
    {
        public static MarketCoinResponse From(MarketCoin c) => new(
            c.Id,
            c.Symbol,
            c.Name,
            c.Image,
            c.CurrentPrice,
            c.MarketCap,
            c.MarketCapRank,
            c.TotalVolume,
            c.High24h,
            c.Low24h,
            c.PriceChangePercentage24h,
            new SparklineResponse(c.Sparkline7d));
    }

    public record SparklineResponse(
        [property: JsonPropertyName("price")] IReadOnlyList<decimal> Price);
}
