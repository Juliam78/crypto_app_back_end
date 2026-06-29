using System.Text.Json;
using System.Text.Json.Serialization;
using CryptoAppBackEnd.Application.Market;
using CryptoAppBackEnd.Application.Ports;
using Microsoft.Extensions.Configuration;

namespace CryptoAppBackEnd.Infraestructure.Market
{
    /// <summary>
    /// Adaptador de salida hacia CoinGecko (implementa <see cref="IMarketDataProvider"/>).
    /// Portado de microservices/MarketService: lista fija de monedas, User-Agent de navegador
    /// (Cloudflare rechaza con 403 las peticiones server-side sin él), backoff exponencial ante
    /// fallos y normalización de campos numéricos null -> 0.
    /// </summary>
    public class CoinGeckoClient : IMarketDataProvider
    {
        private static readonly string[] DefaultCoinIds =
        {
            "bitcoin", "ethereum", "tether", "binancecoin", "solana", "ripple", "usd-coin",
            "staked-ether", "dogecoin", "cardano", "tron", "avalanche-2", "shiba-inu",
            "wrapped-bitcoin", "chainlink", "polkadot", "bitcoin-cash", "near", "litecoin", "uniswap"
        };

        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public CoinGeckoClient(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<IReadOnlyList<MarketCoin>> GetTopCoinsAsync(string currency)
        {
            var coins = await FetchAsync(currency, DefaultCoinIds);
            return coins.Select(Map).ToList();
        }

        public async Task<MarketCoin?> GetCoinAsync(string currency, string coinId)
        {
            var coins = await FetchAsync(currency, new[] { coinId });
            var coin = coins.FirstOrDefault();
            return coin is null ? null : Map(coin);
        }

        private async Task<List<CoinDto>> FetchAsync(string currency, IReadOnlyCollection<string> ids, int attempt = 0)
        {
            var query = new Dictionary<string, string?>
            {
                ["vs_currency"] = currency,
                ["ids"] = string.Join(",", ids),
                ["order"] = "market_cap_desc",
                ["per_page"] = ids.Count.ToString(),
                ["page"] = "1",
                ["sparkline"] = "true",
                ["price_change_percentage"] = "24h",
            };

            var url = "https://api.coingecko.com/api/v3/coins/markets?" +
                      string.Join("&", query.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value ?? string.Empty)}"));

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("accept", "application/json");
                // Cloudflare (delante de CoinGecko) rechaza con 403 las peticiones sin User-Agent.
                request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0 Safari/537.36");
                var apiKey = _config["CoinGecko:ApiKey"];
                if (!string.IsNullOrWhiteSpace(apiKey))
                    request.Headers.Add("x-cg-demo-api-key", apiKey);

                var response = await _http.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"CoinGecko respondió {(int)response.StatusCode}");

                var stream = await response.Content.ReadAsStreamAsync();
                var coins = await JsonSerializer.DeserializeAsync<List<CoinDto>>(stream, JsonOptions) ?? new List<CoinDto>();
                return coins;
            }
            catch when (attempt < 4)
            {
                // Backoff exponencial: 3s, 6s, 12s, máximo 16s.
                var nextDelay = Math.Min(16000, 3000 * (int)Math.Pow(2, attempt));
                await Task.Delay(nextDelay);
                return await FetchAsync(currency, ids, attempt + 1);
            }
        }

        // Mapea el DTO de CoinGecko al modelo de Application, normalizando nulos a 0.
        private static MarketCoin Map(CoinDto dto) => new()
        {
            Id = dto.Id,
            Symbol = dto.Symbol,
            Name = dto.Name,
            Image = dto.Image,
            CurrentPrice = dto.CurrentPrice ?? 0m,
            MarketCap = dto.MarketCap ?? 0m,
            MarketCapRank = dto.MarketCapRank ?? 0,
            TotalVolume = dto.TotalVolume ?? 0m,
            High24h = dto.High24h ?? 0m,
            Low24h = dto.Low24h ?? 0m,
            PriceChangePercentage24h = dto.PriceChangePercentage24h ?? 0m,
            Sparkline7d = dto.SparklineIn7d?.Price ?? new List<decimal>(),
        };

        // DTO de deserialización con las mismas claves que devuelve CoinGecko.
        private sealed class CoinDto
        {
            [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
            [JsonPropertyName("symbol")] public string Symbol { get; set; } = string.Empty;
            [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
            [JsonPropertyName("image")] public string Image { get; set; } = string.Empty;
            [JsonPropertyName("current_price")] public decimal? CurrentPrice { get; set; }
            [JsonPropertyName("market_cap")] public decimal? MarketCap { get; set; }
            [JsonPropertyName("market_cap_rank")] public int? MarketCapRank { get; set; }
            [JsonPropertyName("total_volume")] public decimal? TotalVolume { get; set; }
            [JsonPropertyName("high_24h")] public decimal? High24h { get; set; }
            [JsonPropertyName("low_24h")] public decimal? Low24h { get; set; }
            [JsonPropertyName("price_change_percentage_24h")] public decimal? PriceChangePercentage24h { get; set; }
            [JsonPropertyName("sparkline_in_7d")] public SparklineDto? SparklineIn7d { get; set; }
        }

        private sealed class SparklineDto
        {
            [JsonPropertyName("price")] public List<decimal> Price { get; set; } = new();
        }
    }
}
