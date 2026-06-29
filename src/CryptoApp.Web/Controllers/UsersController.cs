using CryptoApp.Web.Contracts;
using CryptoAppBackEnd.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace CryptoApp.Web.Controllers
{
    /// <summary>
    /// Operaciones sobre usuarios alineadas al contrato del frontend.
    /// Por ahora solo el cambio de rol (resto del CRUD vive en PersonsController).
    /// </summary>
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly AuthUseCase _authUseCase;
        private readonly PersonUseCase _personUseCase;

        public UsersController(AuthUseCase authUseCase, PersonUseCase personUseCase)
        {
            _authUseCase = authUseCase;
            _personUseCase = personUseCase;
        }

        // POST /api/users/{id}/role  -> cambia el rol del usuario {id}.
        // El body trae el id del actor; solo el admin maestro puede cambiar roles.
        [HttpPost("{id:int}/role")]
        public async Task<ActionResult<PersonResponse>> SetRole(int id, [FromBody] SetRoleRequest request)
        {
            // Resuelve el correo del actor a partir de su id (el frontend envía el id).
            if (!int.TryParse(request.ActorUserId, out var actorId))
            {
                return StatusCode(403, new { message = "No autorizado para cambiar roles." });
            }

            var actor = await _personUseCase.GetPersonById(actorId);
            if (actor is null)
            {
                return StatusCode(403, new { message = "No autorizado para cambiar roles." });
            }

            try
            {
                var updated = await _authUseCase.SetRole(id, request.Role, actor.email);
                return Ok(PersonResponse.From(updated));
            }
            catch (UnauthorizedAccessException)
            {
                // NO usar Forbid() sin middleware de auth (lanza 500). Devolver 403 explícito.
                return StatusCode(403, new { message = "No autorizado para cambiar roles." });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
