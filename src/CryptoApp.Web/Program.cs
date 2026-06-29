using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Application.UseCases;
using CryptoAppBackEnd.Infraestructure.Adapters;
using CryptoAppBackEnd.Infraestructure.Market;
using CryptoAppBackEnd.Infraestructure.Persistence;
using CryptoAppBackEnd.Infraestructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// CORS abierto (contexto académico): permite cualquier origen, cabecera y método.
const string CorsPolicy = "AllowAll";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CryptoApp BackEnd API",
        Version = "v1",
        Description = "API para gestionar personas, criptomonedas, portafolios, activos y movimientos."
    });
});

// Persistencia: PostgreSQL con EntityFrameworkCore
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adaptadores (puertos de salida)
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<ICryptoCurrencyRepository, CryptoCurrencyRepository>();
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddScoped<IPortfolioAssetRepository, PortfolioAssetRepository>();
builder.Services.AddScoped<IMovementRepository, MovementRepository>();

// Seguridad: hashing de contraseñas y emisión/validación de tokens de sesión.
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<ITokenService, TokenService>();

// Mercado: proxy a CoinGecko (HttpClient tipado) + caché en BD.
builder.Services.AddHttpClient<IMarketDataProvider, CoinGeckoClient>(client =>
    client.Timeout = TimeSpan.FromSeconds(30));
builder.Services.AddScoped<IMarketCache, MarketCacheRepository>();

// Casos de uso (capa de aplicación)
builder.Services.AddScoped<PersonUseCase>();
builder.Services.AddScoped<CryptoCurrencyUseCase>();
builder.Services.AddScoped<PortfolioUseCase>();
builder.Services.AddScoped<PortfolioAssetUseCase>();
builder.Services.AddScoped<MovementUseCase>();
builder.Services.AddScoped<AuthUseCase>();
builder.Services.AddScoped<MarketUseCase>();

var app = builder.Build();

// Aplica migraciones pendientes y siembra datos iniciales.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    DbInitializer.Seed(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DocumentTitle = "CryptoApp BackEnd API";
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CryptoApp BackEnd API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseCors(CorsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();
