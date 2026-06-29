namespace CryptoAppBackEnd.Infraestructure.Persistence.Models
{
    /// <summary>
    /// Modelo de persistencia (POCO) de la tabla "movements".
    /// </summary>
    public class MovementDbModel
    {
        public int id { get; set; }
        public int person_id { get; set; }
        public int portfolio_id { get; set; }
        public int crypto_id { get; set; }
        public char type { get; set; }
        public decimal quantity { get; set; }
        public decimal price { get; set; }
        public decimal total { get; set; }
        public decimal realized_pnl { get; set; }
        public DateTime created_at { get; set; }
    }
}
