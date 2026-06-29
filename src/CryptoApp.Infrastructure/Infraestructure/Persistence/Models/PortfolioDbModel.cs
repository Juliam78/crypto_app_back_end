namespace CryptoAppBackEnd.Infraestructure.Persistence.Models
{
    /// <summary>
    /// Modelo de persistencia (POCO) de la tabla "portfolios".
    /// </summary>
    public class PortfolioDbModel
    {
        public int id { get; set; }
        public int person_id { get; set; }
        public string name { get; set; } = string.Empty;
        public string base_currency { get; set; } = string.Empty;
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
