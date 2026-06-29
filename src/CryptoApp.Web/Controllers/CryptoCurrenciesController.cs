using CryptoApp.Web.Contracts;
using CryptoAppBackEnd.Application.UseCases;
using CryptoAppBackEnd.Domains.Entities.CryptoCurrencies;
using Microsoft.AspNetCore.Mvc;

namespace CryptoApp.Web.Controllers
{
    [ApiController]
    [Route("api/cryptocurrencies")]
    public class CryptoCurrenciesController : ControllerBase
    {
        private readonly CryptoCurrencyUseCase _useCase;

        public CryptoCurrenciesController(CryptoCurrencyUseCase useCase)
        {
            _useCase = useCase;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CryptoCurrencyResponse>>> GetAll()
        {
            var coins = await _useCase.GetPortfolio();
            return Ok(coins.Select(CryptoCurrencyResponse.From));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CryptoCurrencyResponse>> GetById(int id)
        {
            var coin = await _useCase.GetCryptoCurrencyById(id);
            if (coin is null)
            {
                return NotFound(new { message = $"No existe una criptomoneda con id {id}." });
            }

            return Ok(CryptoCurrencyResponse.From(coin));
        }

        [HttpPost]
        public async Task<ActionResult<CryptoCurrencyResponse>> Create([FromBody] CreateCryptoCurrencyRequest request)
        {
            try
            {
                var coin = new CryptoCurrency(
                    request.Symbol,
                    request.Name,
                    request.CurrentPrice,
                    request.PriceChange24h,
                    request.MarketCap);
                await _useCase.CreateCryptoCurrency(coin);
                return CreatedAtAction(nameof(GetById), new { id = coin.id }, CryptoCurrencyResponse.From(coin));
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CryptoCurrencyResponse>> Update(int id, [FromBody] UpdateCryptoCurrencyRequest request)
        {
            var existing = await _useCase.GetCryptoCurrencyById(id);
            if (existing is null)
            {
                return NotFound(new { message = $"No existe una criptomoneda con id {id}." });
            }

            try
            {
                existing.UpdateDetails(
                    request.Symbol,
                    request.Name,
                    request.ImageUrl,
                    request.CurrentPrice,
                    request.PriceChange24h,
                    request.MarketCap);
                await _useCase.UpdateCryptoCurrency(existing);
                return Ok(CryptoCurrencyResponse.From(existing));
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _useCase.DeleteCryptoCurrency(id);
                return NoContent();
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
