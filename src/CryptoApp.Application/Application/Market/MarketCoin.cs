namespace CryptoAppBackEnd.Application.Market
{
    /// <summary>
    /// Modelo de datos de mercado normalizado para una criptomoneda.
    /// Contiene exactamente los campos que el frontend espera (mismas claves que devuelve CoinGecko
    /// y que servía MarketService). Los campos numéricos ya vienen normalizados (sin nulos).
    /// </summary>
    public class MarketCoin
    {
        public string Id { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
        public decimal MarketCap { get; set; }
        public int MarketCapRank { get; set; }
        public decimal TotalVolume { get; set; }
        public decimal High24h { get; set; }
        public decimal Low24h { get; set; }
        public decimal PriceChangePercentage24h { get; set; }
        public IReadOnlyList<decimal> Sparkline7d { get; set; } = new List<decimal>();
    }
}
