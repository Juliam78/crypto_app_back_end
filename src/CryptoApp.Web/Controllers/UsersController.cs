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
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public UsersController(
            AuthUseCase authUseCase,
            PersonUseCase personUseCase,
            IWebHostEnvironment env,
            IConfiguration config)
        {
            _authUseCase = authUseCase;
            _personUseCase = personUseCase;
            _env = env;
            _config = config;
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

        // POST /api/users/{id}/avatar  (multipart/form-data, campo "file")
        // Guarda el archivo en wwwroot/avatars, actualiza avatar_url y devuelve { url }.
        [HttpPost("{id:int}/avatar")]
        public async Task<IActionResult> UploadAvatar(int id, IFormFile? file)
        {
            if (file is null || file.Length == 0)
            {
                return BadRequest(new { message = "Se esperaba un archivo en el campo 'file'." });
            }

            var person = await _personUseCase.GetPersonById(id);
            if (person is null)
            {
                return NotFound(new { message = "El usuario no existe." });
            }

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext)) ext = ".png";
            var fileName = $"{Guid.NewGuid()}{ext}";

            var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            var folder = Path.Combine(webRoot, "avatars");
            Directory.CreateDirectory(folder);

            await using (var stream = System.IO.File.Create(Path.Combine(folder, fileName)))
            {
                await file.CopyToAsync(stream);
            }

            var publicBase = _config["PublicBaseUrl"] ?? "http://localhost:5000";
            var url = $"{publicBase.TrimEnd('/')}/avatars/{fileName}";

            person.SetAvatarUrl(url);
            await _personUseCase.UpdatePerson(person);

            return Ok(new { url });
        }
    }
}
