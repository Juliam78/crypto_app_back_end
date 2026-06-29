using AdminService;
using AdminService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AdminDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    DbStartup.EnsureCreatedWithRetry(scope.ServiceProvider.GetRequiredService<AdminDbContext>());
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

app.MapPost("/api/errors", async (LogErrorRequest req, AdminDbContext db) =>
{
    var error = new AppError
    {
        route = req.route,
        message = req.message,
        stack = req.stack,
        user_id = req.user_id,
        user_email = req.user_email,
        created_at = DateTime.UtcNow,
    };
    db.AppErrors.Add(error);
    await db.SaveChangesAsync();
    return Results.Ok(AppErrorDto.From(error));
});

app.MapGet("/api/errors", async (AdminDbContext db) =>
{
    var errors = await db.AppErrors.AsNoTracking()
        .OrderByDescending(e => e.created_at)
        .Take(100)
        .ToListAsync();
    return Results.Ok(errors.Select(AppErrorDto.From));
});

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "admin" }));

app.Run();
