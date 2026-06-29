using CryptoApp.Web.Contracts;
using CryptoAppBackEnd.Application.UseCases;
using CryptoAppBackEnd.Domains.Entities.Movements;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovementResponse>>> GetAll()
        {
            var movements = await _useCase.GetMovementsAsync();
            return Ok(movements.Select(MovementResponse.From));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MovementResponse>> GetById(int id)
        {
            var movement = await _useCase.GetMovementsById(id);
            if (movement is null)
            {
                return NotFound(new { message = $"No existe un movimiento con id {id}." });
            }

            return Ok(MovementResponse.From(movement));
        }

        [HttpPost]
        public async Task<ActionResult<MovementResponse>> Create([FromBody] CreateMovementRequest request)
        {
            try
            {
                var movement = new Movement(
                    MovementTypeMapping.ToDomain(request.Type),
                    request.Quantity,
                    request.Price,
                    request.Total,
                    request.RealizedPnl);
                movement.AssignContext(request.PersonId, request.PortfolioId, request.CryptoId);
                await _useCase.CreateMovement(movement);
                return CreatedAtAction(nameof(GetById), new { id = movement.id }, MovementResponse.From(movement));
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
