using Microsoft.EntityFrameworkCore;
using PortfolioService;
using PortfolioService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PortfolioDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    DbStartup.EnsureCreatedWithRetry(scope.ServiceProvider.GetRequiredService<PortfolioDbContext>());
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

// --- Portafolios ---
app.MapGet("/api/portfolios", async (int? personId, PortfolioDbContext db) =>
{
    var query = db.Portfolios.AsNoTracking().Include(p => p.assets).AsQueryable();
    if (personId is not null) query = query.Where(p => p.person_id == personId);
    return Results.Ok(await query.ToListAsync());
});

app.MapGet("/api/portfolios/{id:int}", async (int id, PortfolioDbContext db) =>
{
    var portfolio = await db.Portfolios.AsNoTracking().Include(p => p.assets)
        .FirstOrDefaultAsync(p => p.id == id);
    return portfolio is null ? Results.NotFound() : Results.Ok(portfolio);
});

app.MapPost("/api/portfolios", async (PortfolioRequest req, PortfolioDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(req.name) || string.IsNullOrWhiteSpace(req.base_currency))
        return Results.BadRequest(new { message = "Nombre y moneda base son obligatorios." });

    var portfolio = new Portfolio
    {
        person_id = req.person_id,
        name = req.name,
        base_currency = req.base_currency,
    };
    db.Portfolios.Add(portfolio);
    await db.SaveChangesAsync();
    return Results.Created($"/api/portfolios/{portfolio.id}", portfolio);
});

app.MapPut("/api/portfolios/{id:int}", async (int id, PortfolioRequest req, PortfolioDbContext db) =>
{
    var portfolio = await db.Portfolios.FindAsync(id);
    if (portfolio is null) return Results.NotFound();

    portfolio.name = req.name;
    portfolio.base_currency = req.base_currency;
    portfolio.updated_at = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(portfolio);
});

app.MapDelete("/api/portfolios/{id:int}", async (int id, PortfolioDbContext db) =>
{
    var portfolio = await db.Portfolios.FindAsync(id);
    if (portfolio is null) return Results.NotFound();

    db.Portfolios.Remove(portfolio);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// --- Activos del portafolio ---
app.MapGet("/api/portfolios/{id:int}/assets", async (int id, PortfolioDbContext db) =>
{
    if (!await db.Portfolios.AnyAsync(p => p.id == id)) return Results.NotFound();
    var assets = await db.PortfolioAssets.AsNoTracking().Where(a => a.portfolio_id == id).ToListAsync();
    return Results.Ok(assets);
});

app.MapPost("/api/portfolios/{id:int}/assets", async (int id, AssetRequest req, PortfolioDbContext db) =>
{
    if (!await db.Portfolios.AnyAsync(p => p.id == id)) return Results.NotFound();

    var asset = new PortfolioAsset
    {
        portfolio_id = id,
        coin_id = req.coin_id,
        quantity = req.quantity,
        average_buy_price = req.average_buy_price,
        total_invested = req.total_invested,
    };
    db.PortfolioAssets.Add(asset);
    await db.SaveChangesAsync();
    return Results.Created($"/api/portfolios/{id}/assets/{asset.id}", asset);
});

app.MapDelete("/api/portfolios/{id:int}/assets/{assetId:int}", async (int id, int assetId, PortfolioDbContext db) =>
{
    var asset = await db.PortfolioAssets.FirstOrDefaultAsync(a => a.id == assetId && a.portfolio_id == id);
    if (asset is null) return Results.NotFound();

    db.PortfolioAssets.Remove(asset);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "portfolio" }));

app.Run();
