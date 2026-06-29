using CryptoApp.Web.Contracts;
using CryptoAppBackEnd.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace CryptoApp.Web.Controllers
{
    /// <summary>
    /// Mascota IA asistente ("Cripto"): responde preguntas y sugiere comprar/vender fundamentándose
    /// en datos reales (mercado, tenencias del usuario y señales publicadas). El usuario se identifica
    /// por el token Bearer (igual que AuthController.Me / LessonsController); si no hay token válido,
    /// responde igual pero sin contexto de portafolio. Nunca falla: si el LLM no está disponible,
    /// el caso de uso devuelve una respuesta determinista.
    /// </summary>
    [ApiController]
    [Route("api/assistant")]
    public class AssistantController : ControllerBase
    {
        private readonly AssistantUseCase _useCase;
        private readonly AuthUseCase _authUseCase;

        public AssistantController(AssistantUseCase useCase, AuthUseCase authUseCase)
        {
            _useCase = useCase;
            _authUseCase = authUseCase;
        }

        // POST /api/assistant/ask -> 200 con la respuesta; 400 si la pregunta está vacía.
        [HttpPost("ask")]
        public async Task<ActionResult<AssistantResponse>> Ask([FromBody] AskRequest request, CancellationToken ct)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.question))
            {
                return BadRequest(new { message = "La pregunta no puede estar vacía." });
            }

            var token = ExtractBearerToken();
            var user = await _authUseCase.GetCurrentUser(token); // puede ser null: responde sin portafolio.

            var answer = await _useCase.Ask(request.question, request.coinId, user, ct);
            return Ok(new AssistantResponse(answer.Text, answer.Source));
        }

        private string? ExtractBearerToken()
        {
            var header = Request.Headers.Authorization.ToString();
            if (string.IsNullOrWhiteSpace(header)) return null;
            return header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? header[7..] : header;
        }
    }
}
