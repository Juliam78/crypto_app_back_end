namespace CryptoAppBackEnd.Infraestructure.Persistence.Models
{
    /// <summary>
    /// Modelo de persistencia (POCO) de la tabla "persons".
    /// EF Core mapea esta clase; la traducción Dominio↔DbModel la hacen los mappers.
    /// </summary>
    public class PersonDbModel
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string password_hash { get; set; } = string.Empty;
        public char role { get; set; }
        public bool status { get; set; }
        public string? avatar_url { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
