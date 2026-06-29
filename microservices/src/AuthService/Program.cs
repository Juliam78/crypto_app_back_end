using AuthService;
using AuthService.Data;
using AuthService.Domain;
using AuthService.Security;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<TokenService>();

// CORS abierto (el navegador llega vía gateway; en producción restringir orígenes).
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Crea el esquema si no existe y siembra datos iniciales.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    DbStartup.EnsureCreatedWithRetry(db);
    Seed(db);
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles(); // sirve /avatars/* desde wwwroot
app.UseCors();

// Correo del administrador maestro: único autorizado a cambiar roles.
const string originalAdminEmail = "admin@crypto.edu";

// --- Autenticación ---
app.MapPost("/api/auth/login", async (LoginRequest req, AuthDbContext db, TokenService tokens) =>
{
    var hash = PasswordHasher.Hash(req.password);
    var person = await db.Persons.AsNoTracking()
        .FirstOrDefaultAsync(p => p.email == req.email && p.password_hash == hash);

    return person is null
        ? Results.Unauthorized()
        : Results.Ok(AuthResponse(person, tokens));
});

app.MapPost("/api/auth/register", async (RegisterRequest req, AuthDbContext db, TokenService tokens) =>
{
    if (await db.Persons.AnyAsync(p => p.email == req.email))
        return Results.Conflict(new { message = "Ya existe un usuario con ese correo." });

    var person = new Person
    {
        name = req.name,
        email = req.email,
        password_hash = PasswordHasher.Hash(req.password),
        role = AppUserDto.ToDomainRole(req.role ?? "user"),
        status = true,
    };

    db.Persons.Add(person);
    await db.SaveChangesAsync();
    return Results.Ok(AuthResponse(person, tokens));
});

// Restaura la sesión a partir del token (cabecera Authorization: Bearer ...).
app.MapGet("/api/auth/me", async (HttpRequest request, AuthDbContext db, TokenService tokens) =>
{
    var header = request.Headers.Authorization.ToString();
    var token = header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? header[7..] : header;

    var userId = tokens.Validate(token);
    if (userId is null) return Results.Unauthorized();

    var person = await db.Persons.AsNoTracking().FirstOrDefaultAsync(p => p.id == userId);
    return person is null ? Results.Unauthorized() : Results.Ok(AppUserDto.From(person));
});

// --- Usuarios ---
app.MapGet("/api/users", async (AuthDbContext db) =>
{
    var users = await db.Persons.AsNoTracking()
        .OrderBy(p => p.created_at)
        .ToListAsync();
    return Results.Ok(users.Select(AppUserDto.From));
});

app.MapPut("/api/users/{id:int}", async (int id, UpdateProfileRequest req, AuthDbContext db) =>
{
    var person = await db.Persons.FindAsync(id);
    if (person is null) return Results.NotFound();

    person.name = req.name;
    person.email = req.email;
    person.avatar_url = req.avatar_url;
    if (!string.IsNullOrWhiteSpace(req.password))
        person.password_hash = PasswordHasher.Hash(req.password);
    person.updated_at = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok(AppUserDto.From(person));
});

app.MapPost("/api/users/{id:int}/role", async (int id, SetRoleRequest req, AuthDbContext db) =>
{
    // Solo el admin maestro puede cambiar roles.
    static IResult Forbidden() => Results.Json(new { message = "No autorizado para cambiar roles." }, statusCode: StatusCodes.Status403Forbidden);
    if (!int.TryParse(req.actor_user_id, out var actorId)) return Forbidden();
    var actor = await db.Persons.AsNoTracking().FirstOrDefaultAsync(p => p.id == actorId);
    if (actor is null || actor.email != originalAdminEmail) return Forbidden();

    var person = await db.Persons.FindAsync(id);
    if (person is null) return Results.NotFound();

    person.role = AppUserDto.ToDomainRole(req.role);
    person.updated_at = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(AppUserDto.From(person));
});

// --- Avatares (reemplaza Supabase Storage) ---
app.MapPost("/api/users/{id:int}/avatar", async (int id, HttpRequest request, AuthDbContext db, IConfiguration config) =>
{
    if (!request.HasFormContentType) return Results.BadRequest(new { message = "Se esperaba multipart/form-data." });
    var form = await request.ReadFormAsync();
    var file = form.Files.GetFile("file");
    if (file is null || file.Length == 0) return Results.BadRequest(new { message = "Archivo vacío." });

    var person = await db.Persons.FindAsync(id);
    if (person is null) return Results.NotFound();

    var ext = Path.GetExtension(file.FileName);
    if (string.IsNullOrWhiteSpace(ext)) ext = ".png";
    var fileName = $"{Guid.NewGuid()}{ext}";
    var folder = Path.Combine(app.Environment.WebRootPath ?? "wwwroot", "avatars");
    Directory.CreateDirectory(folder);
    await using (var stream = File.Create(Path.Combine(folder, fileName)))
        await file.CopyToAsync(stream);

    // La URL pública apunta al gateway, que enruta /avatars/* hacia este servicio.
    var publicBase = config["PublicBaseUrl"] ?? "http://localhost:8080";
    var url = $"{publicBase}/avatars/{fileName}";

    person.avatar_url = url;
    person.updated_at = DateTime.UtcNow;
    await db.SaveChangesAsync();

    return Results.Ok(new { url });
});

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "auth" }));

app.Run();

// Respuesta de autenticación: usuario + token de sesión.
static object AuthResponse(Person person, TokenService tokens) => new
{
    user = AppUserDto.From(person),
    token = tokens.Create(person.id, person.email, AppUserDto.MapRole(person.role)),
};

// --- Seed ---
static void Seed(AuthDbContext db)
{
    if (db.Persons.Any()) return;

    db.Persons.AddRange(
        new Person { name = "Admin Maestro", email = "admin@crypto.edu", password_hash = PasswordHasher.Hash("admin123"), role = 'A', status = true },
        new Person { name = "Jane Smith", email = "jane.smith@example.com", password_hash = PasswordHasher.Hash("secret123"), role = 'U', status = true },
        new Person { name = "John Doe", email = "john.doe@example.com", password_hash = PasswordHasher.Hash("secret123"), role = 'U', status = true }
    );
    db.SaveChanges();
}
