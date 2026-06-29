using CryptoAppBackEnd.Domains.Shared;

namespace CryptoAppBackEnd.Domains.Entities.Errors
{
    /// <summary>
    /// Registro de un error capturado por el frontend (réplica del AdminService).
    /// route y message son obligatorios; stack, user_id y user_email son opcionales.
    /// </summary>
    public class AppError
    {
        public int id { get; private set; }
        public string route { get; private set; } = string.Empty;
        public string message { get; private set; } = string.Empty;
        public string? stack { get; private set; }
        public string? user_id { get; private set; }
        public string? user_email { get; private set; }
        public DateTime created_at { get; private set; } = DateTime.UtcNow;

        // Parameterless constructor required by EF Core / design-time materialization.
        private AppError() { }

        /// <summary>
        /// Crea un registro de error nuevo, validado. Solo route y message son obligatorios.
        /// </summary>
        public AppError(
            string route,
            string message,
            string? stack = null,
            string? user_id = null,
            string? user_email = null)
        {
            Helpers.ValidateFields(
                (nameof(route), route),
                (nameof(message), message)
            );

            this.route = route;
            this.message = message;
            this.stack = stack;
            this.user_id = user_id;
            this.user_email = user_email;
            this.created_at = DateTime.UtcNow;
        }

        /// <summary>
        /// Rehidrata un AppError desde almacenamiento (estado completo, sin validación).
        /// </summary>
        public static AppError FromPersistence(
            int id,
            string route,
            string message,
            string? stack,
            string? user_id,
            string? user_email,
            DateTime created_at)
        {
            return new AppError
            {
                id = id,
                route = route,
                message = message,
                stack = stack,
                user_id = user_id,
                user_email = user_email,
                created_at = created_at
            };
        }
    }
}
