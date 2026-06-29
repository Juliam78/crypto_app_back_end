namespace CryptoAppBackEnd.Infraestructure.Persistence.Models
{
    /// <summary>
    /// Modelo de persistencia (POCO) de la tabla "movements". Desnormalizado: sin FKs a otras
    /// tablas; guarda los identificadores y nombres como datos planos.
    /// </summary>
    public class MovementDbModel
    {
        public int id { get; set; }
        public string user_id { get; set; } = string.Empty;
        public string user_name { get; set; } = string.Empty;
        public string coin_id { get; set; } = string.Empty;
        public string coin_name { get; set; } = string.Empty;
        public string coin_symbol { get; set; } = string.Empty;
        public char type { get; set; }
        public decimal quantity { get; set; }
        public string currency { get; set; } = "usd";
        public decimal price { get; set; }
        public decimal total { get; set; }
        public decimal realized_pnl { get; set; }
        public DateTime created_at { get; set; }
    }
}
