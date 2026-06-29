using CryptoAppBackEnd.Application.Market;

namespace CryptoAppBackEnd.Application.Ports
{
    /// <summary>
    /// Puerto de salida hacia el proveedor externo de datos de mercado (CoinGecko).
    /// El adaptador concreto (HttpClient con backoff) vive en Infrastructure.
    /// </summary>
    public interface IMarketDataProvider
    {
        /// <summary>Lista de las monedas principales en la divisa indicada. Lanza excepción si el proveedor falla.</summary>
        Task<IReadOnlyList<MarketCoin>> GetTopCoinsAsync(string currency);

        /// <summary>Detalle de una moneda; null si no existe. Lanza excepción si el proveedor falla.</summary>
        Task<MarketCoin?> GetCoinAsync(string currency, string coinId);
    }
}
