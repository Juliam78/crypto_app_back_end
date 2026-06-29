using CryptoApp.Web.Contracts;
using CryptoAppBackEnd.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace CryptoApp.Web.Controllers
{
    /// <summary>
    /// Proxy de mercado hacia CoinGecko con caché. Si el proveedor falla y no hay
    /// caché, responde 503 (no 500), igual que el MarketService de microservicios.
    /// </summary>
    [ApiController]
    [Route("api/market")]
    public class MarketController : ControllerBase
    {
        private readonly MarketUseCase _useCase;

        public MarketController(MarketUseCase useCase)
        {
            _useCase = useCase;
        }

        [HttpGet("coins")]
        public async Task<IActionResult> GetCoins([FromQuery] string currency = "usd")
        {
            var result = await _useCase.GetTopCoinsAsync(currency);
            if (!result.Available)
                return StatusCode(503, new { message = "Mercado no disponible y sin datos en caché." });

            return Ok(result.Value.Select(MarketCoinResponse.From));
        }

        [HttpGet("coins/{coinId}")]
        public async Task<IActionResult> GetCoin(string coinId, [FromQuery] string currency = "usd")
        {
            var result = await _useCase.GetCoinAsync(currency, coinId);
            if (!result.Available)
                return StatusCode(503, new { message = "Mercado no disponible y sin datos en caché." });
            if (result.Value is null)
                return NotFound(new { message = "Moneda no encontrada." });

            return Ok(MarketCoinResponse.From(result.Value));
        }
    }
}
