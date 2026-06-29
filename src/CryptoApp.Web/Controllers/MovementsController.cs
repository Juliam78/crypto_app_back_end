using CryptoApp.Web.Contracts;
using CryptoAppBackEnd.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace CryptoApp.Web.Controllers
{
    [ApiController]
    [Route("api/movements")]
    public class MovementsController : ControllerBase
    {
        private readonly MovementUseCase _useCase;

        public MovementsController(MovementUseCase useCase)
        {
            _useCase = useCase;
        }

        /// <summary>
        /// Historial de movimientos. role=user filtra por su userId; admin (u otro) ve todo.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovementResponse>>> Get(
            [FromQuery] string? userId,
            [FromQuery] string? role)
        {
            var movements = await _useCase.GetMovements(userId, role);
            return Ok(movements.Select(MovementResponse.From));
        }
    }
}
