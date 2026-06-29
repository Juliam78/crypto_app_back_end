namespace CryptoAppBackEnd.Infraestructure.Persistence.Models
{
    /// <summary>
    /// Modelo de persistencia (POCO) de la tabla "lessons". Entidad única para lecciones ('L')
    /// y señales ('S'). Los campos de señal (coin_id/coin_symbol/recommendation) son nullables.
    /// </summary>
    public class LessonDbModel
    {
        public int id { get; set; }
        public char kind { get; set; }
        public string title { get; set; } = string.Empty;
        public string body { get; set; } = string.Empty;
        public string? coin_id { get; set; }
        public string? coin_symbol { get; set; }
        public char? recommendation { get; set; }
        public string author_id { get; set; } = string.Empty;
        public string author_name { get; set; } = string.Empty;
        public bool published { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
