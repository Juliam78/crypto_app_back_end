using CryptoAppBackEnd.Application.Market;

namespace CryptoAppBackEnd.Application.Ports
{
    /// <summary>
    /// Puerto de salida para la caché de datos de mercado en BD.
    /// Permite servir el último snapshot conocido cuando el proveedor externo no está disponible.
    /// </summary>
    public interface IMarketCache
    {
        /// <summary>Persiste/actualiza el snapshot de monedas para la divisa indicada.</summary>
        Task SaveAsync(string currency, IReadOnlyList<MarketCoin> coins);

        /// <summary>Carga el último snapshot cacheado para la divisa (lista vacía si no hay datos).</summary>
        Task<IReadOnlyList<MarketCoin>> LoadAsync(string currency);
    }
}
