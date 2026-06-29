using System.Text.Json;

namespace MarketService.Services;

/// <summary>
/// Cliente de CoinGecko del lado servidor. Replica la lógica del antiguo coingecko.ts del frontend:
/// lista fija de monedas y backoff exponencial ante fallos.
/// </summary>
public class CoinGeckoClient
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

    public Task<List<Coin>> GetTopCoinsAsync(string currency) =>
        FetchAsync(currency, DefaultCoinIds);

    public async Task<Coin?> GetCoinAsync(string currency, string id)
    {
        var coins = await FetchAsync(currency, new[] { id });
        return coins.FirstOrDefault();
    }

    private async Task<List<Coin>> FetchAsync(string currency, IReadOnlyCollection<string> ids, int attempt = 0)
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
            var coins = await JsonSerializer.DeserializeAsync<List<Coin>>(stream, JsonOptions) ?? new List<Coin>();
            foreach (var coin in coins) coin.Normalize();
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
}
