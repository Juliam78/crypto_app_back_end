using CryptoApp.Web.Contracts;
using CryptoAppBackEnd.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace CryptoApp.Web.Controllers
{
    [ApiController]
    [Route("api/trades")]
    public class TradesController : ControllerBase
    {
        private readonly MovementUseCase _useCase;

        public TradesController(MovementUseCase useCase)
        {
            _useCase = useCase;
        }

        /// <summary>
        /// Registra una compra/venta con cálculo de cantidad y PnL realizado del lado servidor.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<MovementResponse>> Create([FromBody] TradeRequest request)
        {
            try
            {
                var command = new TradeCommand(
                    request.userId,
                    request.userName,
                    request.coinId,
                    request.coinName,
                    request.coinSymbol,
                    request.type,
                    request.amountUsd,
                    request.priceUsd,
                    request.currency);

                var movement = await _useCase.RegisterTrade(command);
                return Ok(MovementResponse.From(movement));
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
