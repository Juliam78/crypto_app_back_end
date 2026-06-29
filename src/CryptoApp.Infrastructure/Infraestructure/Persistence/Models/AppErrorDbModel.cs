namespace CryptoAppBackEnd.Infraestructure.Persistence.Models
{
    /// <summary>
    /// Modelo de persistencia (POCO) de la tabla "app_errors".
    /// </summary>
    public class AppErrorDbModel
    {
        public int id { get; set; }
        public string route { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public string? stack { get; set; }
        public string? user_id { get; set; }
        public string? user_email { get; set; }
        public DateTime created_at { get; set; }
    }
}
