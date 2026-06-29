namespace CryptoAppBackEnd.Infraestructure.Persistence.Models
{
    /// <summary>
    /// Caché de datos de mercado: un snapshot por divisa que guarda el JSON completo de las
    /// monedas (lista normalizada de <c>MarketCoin</c> serializada). Permite servir desde caché
    /// con plena fidelidad cuando CoinGecko no está disponible.
    /// </summary>
    public class CryptoPriceDbModel
    {
        public int id { get; set; }
        public string currency { get; set; } = "usd";
        // JSON completo de la lista de monedas para esta divisa.
        public string payload { get; set; } = string.Empty;
        public DateTime last_updated { get; set; } = DateTime.UtcNow;
    }
}
