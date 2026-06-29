using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Application.UseCases;
using CryptoAppBackEnd.Infraestructure.Adapters;
using CryptoAppBackEnd.Infraestructure.Persistence;
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

// Casos de uso (capa de aplicación)
builder.Services.AddScoped<PersonUseCase>();
builder.Services.AddScoped<CryptoCurrencyUseCase>();
builder.Services.AddScoped<PortfolioUseCase>();
builder.Services.AddScoped<PortfolioAssetUseCase>();
builder.Services.AddScoped<MovementUseCase>();

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
