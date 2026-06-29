using CryptoAppBackEnd.Application.Market;
using CryptoAppBackEnd.Application.Ports;

namespace CryptoAppBackEnd.Application.UseCases
{
    /// <summary>
    /// Caso de uso de mercado: actúa como proxy a CoinGecko con caché en BD.
    /// Si el proveedor responde, refresca la caché y devuelve sus datos; si falla,
    /// sirve el último snapshot cacheado. Si tampoco hay caché, señaliza "no disponible".
    /// </summary>
    public class MarketUseCase
    {
        private readonly IMarketDataProvider _provider;
        private readonly IMarketCache _cache;

        public MarketUseCase(IMarketDataProvider provider, IMarketCache cache)
        {
            _provider = provider;
            _cache = cache;
        }

        /// <summary>
        /// Lista de monedas principales. <see cref="MarketResult{T}.Available"/> es false
        /// solo cuando el proveedor falla y no hay nada en caché.
        /// </summary>
        public async Task<MarketResult<IReadOnlyList<MarketCoin>>> GetTopCoinsAsync(string currency)
        {
            try
            {
                var coins = await _provider.GetTopCoinsAsync(currency);
                await _cache.SaveAsync(currency, coins);
                return MarketResult<IReadOnlyList<MarketCoin>>.Ok(coins);
            }
            catch
            {
                var cached = await _cache.LoadAsync(currency);
                return cached.Count > 0
                    ? MarketResult<IReadOnlyList<MarketCoin>>.Ok(cached)
                    : MarketResult<IReadOnlyList<MarketCoin>>.Unavailable();
            }
        }

        /// <summary>
        /// Detalle de una moneda. Si el proveedor falla, busca la moneda en la caché.
        /// <see cref="MarketResult{T}.Available"/> es false cuando el proveedor falla y la
        /// moneda no está cacheada; <c>Value</c> null indica que la moneda no existe.
        /// </summary>
        public async Task<MarketResult<MarketCoin?>> GetCoinAsync(string currency, string coinId)
        {
            try
            {
                var coin = await _provider.GetCoinAsync(currency, coinId);
                if (coin is not null)
                {
                    await _cache.SaveAsync(currency, new[] { coin });
                }
                return MarketResult<MarketCoin?>.Ok(coin);
            }
            catch
            {
                var cached = await _cache.LoadAsync(currency);
                var match = cached.FirstOrDefault(c => c.Id == coinId);
                return match is not null
                    ? MarketResult<MarketCoin?>.Ok(match)
                    : MarketResult<MarketCoin?>.Unavailable();
            }
        }
    }

    /// <summary>
    /// Resultado de una operación de mercado. <c>Available == false</c> indica que el
    /// mercado no está disponible y no hay datos en caché (el Web responde 503).
    /// </summary>
    public class MarketResult<T>
    {
        public bool Available { get; private init; }
        public T Value { get; private init; } = default!;

        public static MarketResult<T> Ok(T value) => new() { Available = true, Value = value };
        public static MarketResult<T> Unavailable() => new() { Available = false };
    }
}
