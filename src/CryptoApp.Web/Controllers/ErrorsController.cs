using CryptoApp.Web.Contracts;
using CryptoAppBackEnd.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace CryptoApp.Web.Controllers
{
    /// <summary>
    /// Registro y consulta de errores reportados por el frontend (réplica del AdminService).
    /// </summary>
    [ApiController]
    [Route("api/errors")]
    public class ErrorsController : ControllerBase
    {
        private readonly ErrorUseCase _useCase;

        public ErrorsController(ErrorUseCase useCase)
        {
            _useCase = useCase;
        }

        // POST /api/errors -> registra un error (201 Created).
        [HttpPost]
        public async Task<ActionResult<ErrorResponse>> Log([FromBody] LogErrorRequest request)
        {
            var error = await _useCase.LogError(
                request.route,
                request.message,
                request.stack,
                request.userId,
                request.userEmail);

            var response = ErrorResponse.From(error);
            return CreatedAtAction(nameof(GetAll), new { }, response);
        }

        // GET /api/errors -> lista de errores ordenados por fecha descendente.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ErrorResponse>>> GetAll()
        {
            var errors = await _useCase.GetErrors();
            return Ok(errors.Select(ErrorResponse.From));
        }
    }
}
