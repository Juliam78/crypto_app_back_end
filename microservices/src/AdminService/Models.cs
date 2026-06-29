namespace AdminService;

public class AppError
{
    public int id { get; set; }
    public string route { get; set; } = string.Empty;
    public string message { get; set; } = string.Empty;
    public string? stack { get; set; }
    public string? user_id { get; set; }
    public string? user_email { get; set; }
    public DateTime created_at { get; set; } = DateTime.UtcNow;
}

/// <summary>DTO con el shape que espera el frontend (shared/types.ts -> AppErrorLog).</summary>
public record AppErrorDto(
    string id,
    string route,
    string message,
    string? stack,
    string? user_id,
    string? user_email,
    string created_at)
{
    public static AppErrorDto From(AppError e) => new(
        e.id.ToString(), e.route, e.message, e.stack, e.user_id, e.user_email, e.created_at.ToString("o"));
}

public record LogErrorRequest(string route, string message, string? stack, string? user_id, string? user_email);
