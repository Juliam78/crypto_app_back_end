using CryptoAppBackEnd.Domains.Entities.Errors;

namespace CryptoApp.Web.Contracts
{
    /// <summary>
    /// Solicitud de registro de error enviada por el frontend.
    /// route y message son obligatorios; el resto opcional.
    /// </summary>
    public record LogErrorRequest(
        string route,
        string message,
        string? stack,
        string? user_id,
        string? user_email);

    /// <summary>
    /// DTO con el shape que espera el frontend (shared/types.ts -> AppErrorLog).
    /// id se serializa como string; created_at en ISO 8601.
    /// </summary>
    public record ErrorResponse(
        string id,
        string route,
        string message,
        string? stack,
        string? user_id,
        string? user_email,
        string created_at)
    {
        public static ErrorResponse From(AppError e) => new(
            e.id.ToString(),
            e.route,
            e.message,
            e.stack,
            e.user_id,
            e.user_email,
            e.created_at.ToString("o"));
    }
}
