using CryptoApp.Web.Contracts;
using CryptoAppBackEnd.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace CryptoApp.Web.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthUseCase _useCase;

        public AuthController(AuthUseCase useCase)
        {
            _useCase = useCase;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _useCase.Login(request.Email, request.Password);
                return Ok(new AuthResponse(PersonResponse.From(result.Person), result.Token));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Credenciales inválidas." });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _useCase.Register(request.Name, request.Email, request.Password, request.Role);
                return Ok(new AuthResponse(PersonResponse.From(result.Person), result.Token));
            }
            catch (InvalidOperationException ex)
            {
                // Email duplicado.
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex) when (ex is ArgumentException)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("me")]
        public async Task<ActionResult<PersonResponse>> Me()
        {
            var token = ExtractBearerToken();
            var person = await _useCase.GetCurrentUser(token);
            if (person is null)
            {
                return Unauthorized(new { message = "Token inválido o ausente." });
            }

            return Ok(PersonResponse.From(person));
        }

        private string? ExtractBearerToken()
        {
            var header = Request.Headers.Authorization.ToString();
            if (string.IsNullOrWhiteSpace(header)) return null;
            return header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? header[7..] : header;
        }
    }
}
