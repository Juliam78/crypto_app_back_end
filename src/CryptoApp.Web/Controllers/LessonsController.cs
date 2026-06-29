using CryptoApp.Web.Contracts;
using CryptoAppBackEnd.Application.UseCases;
using CryptoAppBackEnd.Domains.Entities.Persons;
using Microsoft.AspNetCore.Mvc;

namespace CryptoApp.Web.Controllers
{
    /// <summary>
    /// Módulo académico: lecciones y señales (entidad única Lesson).
    /// Lectura pública para lo publicado; gestión solo para staff (admin 'A' o empleado 'E'),
    /// autorizado por el token Bearer (no hay middleware de auth: se valida con AuthUseCase).
    /// </summary>
    [ApiController]
    [Route("api/lessons")]
    public class LessonsController : ControllerBase
    {
        private readonly LessonUseCase _useCase;
        private readonly AuthUseCase _authUseCase;

        public LessonsController(LessonUseCase useCase, AuthUseCase authUseCase)
        {
            _useCase = useCase;
            _authUseCase = authUseCase;
        }

        // GET /api/lessons -> staff ve todas; público (o no-staff) ve solo las publicadas.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LessonResponse>>> Get()
        {
            var staff = await GetStaffUser();
            var lessons = staff is not null
                ? await _useCase.GetAll()
                : await _useCase.GetPublished();
            return Ok(lessons.Select(LessonResponse.From));
        }

        // GET /api/lessons/{id} -> 404 si no existe; si no está publicada y no eres staff, 404.
        [HttpGet("{id:int}")]
        public async Task<ActionResult<LessonResponse>> GetById(int id)
        {
            var lesson = await _useCase.GetById(id);
            if (lesson is null)
            {
                return NotFound(new { message = "La lección no existe." });
            }

            if (!lesson.published)
            {
                var staff = await GetStaffUser();
                if (staff is null)
                {
                    return NotFound(new { message = "La lección no existe." });
                }
            }

            return Ok(LessonResponse.From(lesson));
        }

        // POST /api/lessons -> solo staff. La autoría se toma del usuario autenticado.
        [HttpPost]
        public async Task<ActionResult<LessonResponse>> Create([FromBody] CreateLessonRequest request)
        {
            var staff = await GetStaffUser();
            if (staff is null)
            {
                return StatusCode(403, new { message = "Solo el personal puede gestionar lecciones." });
            }

            try
            {
                var command = new LessonCommand(
                    LessonMapping.KindToDomain(request.kind),
                    request.title,
                    request.body,
                    request.coinId,
                    request.coinSymbol,
                    LessonMapping.RecommendationToDomain(request.recommendation));

                var lesson = await _useCase.Create(command, staff.id.ToString(), staff.name);
                return CreatedAtAction(nameof(GetById), new { id = lesson.id }, LessonResponse.From(lesson));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/lessons/{id} -> solo staff.
        [HttpPut("{id:int}")]
        public async Task<ActionResult<LessonResponse>> Update(int id, [FromBody] UpdateLessonRequest request)
        {
            var staff = await GetStaffUser();
            if (staff is null)
            {
                return StatusCode(403, new { message = "Solo el personal puede gestionar lecciones." });
            }

            try
            {
                var command = new LessonCommand(
                    Kind: default,   // UpdateContent no cambia kind; se ignora.
                    request.title,
                    request.body,
                    request.coinId,
                    request.coinSymbol,
                    LessonMapping.RecommendationToDomain(request.recommendation));

                var lesson = await _useCase.Update(id, command);
                if (lesson is null)
                {
                    return NotFound(new { message = "La lección no existe." });
                }

                return Ok(LessonResponse.From(lesson));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE /api/lessons/{id} -> solo staff.
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var staff = await GetStaffUser();
            if (staff is null)
            {
                return StatusCode(403, new { message = "Solo el personal puede gestionar lecciones." });
            }

            var deleted = await _useCase.Delete(id);
            if (!deleted)
            {
                return NotFound(new { message = "La lección no existe." });
            }

            return NoContent();
        }

        // POST /api/lessons/{id}/publish -> solo staff.
        [HttpPost("{id:int}/publish")]
        public async Task<ActionResult<LessonResponse>> Publish(int id)
        {
            var staff = await GetStaffUser();
            if (staff is null)
            {
                return StatusCode(403, new { message = "Solo el personal puede gestionar lecciones." });
            }

            var lesson = await _useCase.Publish(id);
            if (lesson is null)
            {
                return NotFound(new { message = "La lección no existe." });
            }

            return Ok(LessonResponse.From(lesson));
        }

        // POST /api/lessons/{id}/unpublish -> solo staff.
        [HttpPost("{id:int}/unpublish")]
        public async Task<ActionResult<LessonResponse>> Unpublish(int id)
        {
            var staff = await GetStaffUser();
            if (staff is null)
            {
                return StatusCode(403, new { message = "Solo el personal puede gestionar lecciones." });
            }

            var lesson = await _useCase.Unpublish(id);
            if (lesson is null)
            {
                return NotFound(new { message = "La lección no existe." });
            }

            return Ok(LessonResponse.From(lesson));
        }

        /// <summary>
        /// Devuelve la persona autenticada si es staff (rol 'A' admin o 'E' empleado), o null
        /// en cualquier otro caso (token ausente/inválido o usuario sin permisos).
        /// </summary>
        private async Task<Person?> GetStaffUser()
        {
            var token = ExtractBearerToken();
            var person = await _authUseCase.GetCurrentUser(token);
            if (person is null) return null;
            return person.role is 'A' or 'E' ? person : null;
        }

        private string? ExtractBearerToken()
        {
            var header = Request.Headers.Authorization.ToString();
            if (string.IsNullOrWhiteSpace(header)) return null;
            return header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? header[7..] : header;
        }
    }
}
