using CryptoApp.Web.Contracts;
using CryptoAppBackEnd.Application.Ports;
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
        private const long MaxAvatarBytes = 5 * 1024 * 1024; // 5 MB

        private readonly AuthUseCase _authUseCase;
        private readonly PersonUseCase _personUseCase;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAvatarStore _avatarStore;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public UsersController(
            AuthUseCase authUseCase,
            PersonUseCase personUseCase,
            IPasswordHasher passwordHasher,
            IAvatarStore avatarStore,
            IWebHostEnvironment env,
            IConfiguration config)
        {
            _authUseCase = authUseCase;
            _personUseCase = personUseCase;
            _passwordHasher = passwordHasher;
            _avatarStore = avatarStore;
            _env = env;
            _config = config;
        }

        // GET /api/users  -> lista de usuarios (consumido por la vista de administración).
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonResponse>>> GetAll()
        {
            var persons = await _personUseCase.GetPersons();
            return Ok(persons.Select(PersonResponse.From));
        }

        // PUT /api/users/{id}  -> actualiza el perfil (nombre, email, y opcionalmente
        // contraseña y avatar). NO cambia rol ni estado.
        [HttpPut("{id:int}")]
        public async Task<ActionResult<PersonResponse>> UpdateProfile(int id, [FromBody] UpdateProfileRequest request)
        {
            var existing = await _personUseCase.GetPersonById(id);
            if (existing is null)
            {
                return NotFound(new { message = "El usuario no existe." });
            }

            try
            {
                existing.UpdateProfile(request.Name, request.Email, existing.role, existing.status);
                if (!string.IsNullOrWhiteSpace(request.Password))
                {
                    existing.SetPasswordHash(_passwordHasher.Hash(request.Password));
                }
                if (!string.IsNullOrWhiteSpace(request.avatar_url))
                {
                    existing.SetAvatarUrl(request.avatar_url);
                }

                await _personUseCase.UpdatePerson(existing);
                return Ok(PersonResponse.From(existing));
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST /api/users/{id}/role  -> cambia el rol del usuario {id}.
        // El body trae el id del actor; solo el admin maestro puede cambiar roles.
        [HttpPost("{id:int}/role")]
        public async Task<ActionResult<PersonResponse>> SetRole(int id, [FromBody] SetRoleRequest request)
        {
            // Resuelve el correo del actor a partir de su id (el frontend envía el id).
            if (!int.TryParse(request.actor_user_id, out var actorId))
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
        // Guarda la imagen como binario en BD (tabla avatars), actualiza avatar_url apuntando al
        // endpoint GET y devuelve { url }. La URL lleva ?v= para romper la caché del navegador al
        // re-subir. Ya NO se escribe en wwwroot/avatars.
        [HttpPost("{id:int}/avatar")]
        public async Task<IActionResult> UploadAvatar(int id, IFormFile? file, CancellationToken ct)
        {
            if (file is null || file.Length == 0)
            {
                return BadRequest(new { message = "Se esperaba un archivo en el campo 'file'." });
            }

            if (file.Length > MaxAvatarBytes)
            {
                return BadRequest(new { message = "La imagen supera el tamaño máximo de 5 MB." });
            }

            if (!IsImage(file))
            {
                return BadRequest(new { message = "El archivo debe ser una imagen (jpg, png, gif o webp)." });
            }

            var person = await _personUseCase.GetPersonById(id);
            if (person is null)
            {
                return NotFound(new { message = "El usuario no existe." });
            }

            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms, ct);
                bytes = ms.ToArray();
            }

            var contentType = string.IsNullOrWhiteSpace(file.ContentType) ? "image/png" : file.ContentType;
            await _avatarStore.SaveAsync(id, bytes, contentType, ct);

            var publicBase = (_config["PublicBaseUrl"] ?? "http://localhost:5000").TrimEnd('/');
            var url = $"{publicBase}/api/users/{id}/avatar?v={DateTime.UtcNow.Ticks}";

            person.SetAvatarUrl(url);
            await _personUseCase.UpdatePerson(person);

            return Ok(new { url });
        }

        // GET /api/users/{id}/avatar  -> sirve la imagen del avatar desde BD.
        // PÚBLICO (sin token): lo consumen etiquetas <img>, que no envían Authorization.
        // El query ?v= (cache-bust) se acepta y se descarta.
        [HttpGet("{id:int}/avatar")]
        public async Task<IActionResult> GetAvatar(int id, CancellationToken ct)
        {
            var avatar = await _avatarStore.GetAsync(id, ct);
            if (avatar is null)
            {
                return NotFound();
            }

            return File(avatar.Data, avatar.ContentType);
        }

        // Valida que el archivo subido sea una imagen, por content type o por extensión.
        private static bool IsImage(IFormFile file)
        {
            if (!string.IsNullOrWhiteSpace(file.ContentType) &&
                file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var ext = Path.GetExtension(file.FileName);
            return ext.ToLowerInvariant() is ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp";
        }
    }
}
