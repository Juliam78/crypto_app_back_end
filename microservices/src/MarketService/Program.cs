using System.Text.Json;
using MarketService;
using MarketService.Data;
using MarketService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MarketDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<CoinGeckoClient>(c => c.Timeout = TimeSpan.FromSeconds(30));

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    DbStartup.EnsureCreatedWithRetry(scope.ServiceProvider.GetRequiredService<MarketDbContext>());
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

// Lista de precios (proxy + caché). Si CoinGecko falla (p. ej. rate limit / 403),
// se responde con el último snapshot cacheado en lugar de un 500.
app.MapGet("/api/market/coins", async (string? currency, CoinGeckoClient client, MarketDbContext db, ILogger<Program> log) =>
{
    var cur = string.IsNullOrWhiteSpace(currency) ? "usd" : currency;
    try
    {
        var coins = await client.GetTopCoinsAsync(cur);
        await CacheAsync(db, coins, cur);
        return Results.Ok(coins);
    }
    catch (Exception ex)
    {
        log.LogWarning(ex, "CoinGecko no disponible; sirviendo desde caché.");
        var cached = await LoadCacheAsync(db, cur);
        return cached.Count > 0
            ? Results.Ok(cached)
            : Results.Json(new { message = "Mercado no disponible y sin datos en caché." }, statusCode: 503);
    }
});

// Detalle de una moneda (con el mismo fallback a caché).
app.MapGet("/api/market/coins/{id}", async (string id, string? currency, CoinGeckoClient client, MarketDbContext db, ILogger<Program> log) =>
{
    var cur = string.IsNullOrWhiteSpace(currency) ? "usd" : currency;
    try
    {
        var coin = await client.GetCoinAsync(cur, id);
        if (coin is null) return Results.NotFound();
        await CacheAsync(db, new[] { coin }, cur);
        return Results.Ok(coin);
    }
    catch (Exception ex)
    {
        log.LogWarning(ex, "CoinGecko no disponible; sirviendo {Coin} desde caché.", id);
        var cached = (await LoadCacheAsync(db, cur)).FirstOrDefault(c => c.Id == id);
        return cached is not null
            ? Results.Ok(cached)
            : Results.Json(new { message = "Moneda no disponible y sin datos en caché." }, statusCode: 503);
    }
});

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "market" }));

app.Run();

// Persiste/actualiza el snapshot de precios en la BD del servicio.
static async Task CacheAsync(MarketDbContext db, IEnumerable<Coin> coins, string currency)
{
    foreach (var coin in coins)
    {
        var existing = await db.CryptoPrices
            .FirstOrDefaultAsync(c => c.coingecko_id == coin.Id && c.currency == currency);

        var json = JsonSerializer.Serialize(coin);

        if (existing is null)
        {
            db.CryptoPrices.Add(new CryptoPrice
            {
                coingecko_id = coin.Id,
                symbol = coin.Symbol,
                name = coin.Name,
                image_url = coin.Image,
                currency = currency,
                current_price = coin.CurrentPrice ?? 0m,
                price_change_24h = coin.PriceChangePercentage24h ?? 0m,
                market_cap = coin.MarketCap ?? 0m,
                payload = json,
                last_updated = DateTime.UtcNow,
            });
        }
        else
        {
            existing.current_price = coin.CurrentPrice ?? 0m;
            existing.price_change_24h = coin.PriceChangePercentage24h ?? 0m;
            existing.market_cap = coin.MarketCap ?? 0m;
            existing.image_url = coin.Image;
            existing.payload = json;
            existing.last_updated = DateTime.UtcNow;
        }
    }

    await db.SaveChangesAsync();
}

// Reconstruye la lista de monedas desde la caché (orden por capitalización).
static async Task<List<Coin>> LoadCacheAsync(MarketDbContext db, string currency)
{
    var rows = await db.CryptoPrices.AsNoTracking()
        .Where(c => c.currency == currency && c.payload != "")
        .OrderByDescending(c => c.market_cap)
        .ToListAsync();

    var coins = new List<Coin>();
    foreach (var row in rows)
    {
        var coin = JsonSerializer.Deserialize<Coin>(row.payload);
        if (coin is not null) coins.Add(coin);
    }
    return coins;
}
