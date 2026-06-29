using Microsoft.EntityFrameworkCore;
using TradingService;
using TradingService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TradingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    DbStartup.EnsureCreatedWithRetry(scope.ServiceProvider.GetRequiredService<TradingDbContext>());
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

// Historial de movimientos. role=user filtra por su userId; admin (u otro) ve todo.
app.MapGet("/api/movements", async (string? userId, string? role, TradingDbContext db) =>
{
    var query = db.Movements.AsNoTracking().AsQueryable();
    if (role == "user" && !string.IsNullOrWhiteSpace(userId))
        query = query.Where(m => m.user_id == userId);

    var movements = await query.OrderByDescending(m => m.created_at).ToListAsync();
    return Results.Ok(movements.Select(MovementDto.From));
});

// Registrar compra/venta con cálculo de cantidad y PnL realizado del lado servidor.
app.MapPost("/api/trades", async (TradeRequest req, TradingDbContext db) =>
{
    if (req.amountUsd <= 0 || req.priceUsd <= 0)
        return Results.BadRequest(new { message = "Operación inválida: monto y precio deben ser mayores a 0." });

    var currency = string.IsNullOrWhiteSpace(req.currency) ? "usd" : req.currency;
    var isSell = req.type == "sell";
    var quantity = req.amountUsd / req.priceUsd;

    // Costo promedio actual de la posición (réplica de lib/portfolio.ts getAverageCost).
    var history = await db.Movements.AsNoTracking()
        .Where(m => m.user_id == req.userId && m.coin_id == req.coinId && m.currency == currency)
        .OrderBy(m => m.created_at)
        .ToListAsync();

    var (heldQuantity, averageCost) = ComputePosition(history);

    if (isSell && quantity > heldQuantity)
        return Results.BadRequest(new { message = "No tienes saldo suficiente para vender esa cantidad." });

    var realizedPnl = isSell ? (req.priceUsd - averageCost) * quantity : 0m;

    var movement = new Movement
    {
        user_id = req.userId,
        user_name = req.userName ?? string.Empty,
        coin_id = req.coinId,
        coin_name = req.coinName,
        coin_symbol = req.coinSymbol,
        type = isSell ? 'S' : 'B',
        quantity = quantity,
        currency = currency,
        price = req.priceUsd,
        total = req.amountUsd,
        realized_pnl = realizedPnl,
        created_at = DateTime.UtcNow,
    };

    db.Movements.Add(movement);
    await db.SaveChangesAsync();
    return Results.Ok(MovementDto.From(movement));
});

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "trading" }));

app.Run();

// Cantidad retenida y costo promedio ponderado sobre movimientos ordenados por fecha.
static (decimal quantity, decimal averageCost) ComputePosition(IEnumerable<Movement> sorted)
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
