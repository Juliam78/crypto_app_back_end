using Microsoft.EntityFrameworkCore;

/// <summary>
/// Inicializa el esquema tolerando la carrera de arranque contra PostgreSQL:
/// si la BD aún no acepta conexiones, reintenta en vez de tumbar el servicio.
/// </summary>
public static class DbStartup
{
    public static void EnsureCreatedWithRetry(DbContext db, int maxAttempts = 12, int delayMs = 3000)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                db.Database.EnsureCreated();
                return;
            }
            catch when (attempt < maxAttempts)
            {
                Console.WriteLine($"[DbStartup] BD no disponible (intento {attempt}/{maxAttempts}). Reintentando en {delayMs}ms...");
                Thread.Sleep(delayMs);
            }
        }
    }
}
