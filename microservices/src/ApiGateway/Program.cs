var builder = WebApplication.CreateBuilder(args);

// Reverse proxy YARP: la configuración de rutas/clusters vive en appsettings.json.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// CORS para el frontend (Vite dev server). Único punto que el navegador necesita.
const string corsPolicy = "frontend";
builder.Services.AddCors(options => options.AddPolicy(corsPolicy, policy =>
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors(corsPolicy);

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "gateway" }));

app.MapReverseProxy();

app.Run();
