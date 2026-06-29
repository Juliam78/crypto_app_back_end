using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.Academy;

namespace CryptoAppBackEnd.Application.UseCases
{
    /// <summary>Datos para crear/actualizar una lección o señal (kind/recommendation ya en char de dominio).</summary>
    public record LessonCommand(
        char Kind,
        string Title,
        string Body,
        string? CoinId,
        string? CoinSymbol,
        char? Recommendation);

    /// <summary>
    /// Caso de uso del módulo académico. Cubre lecciones ('L') y señales ('S') con una sola entidad.
    /// Las reglas de negocio (señal requiere coin + recommendation) viven en la entidad Lesson.
    /// </summary>
    public class LessonUseCase
    {
        private readonly ILessonRepository _lessonPort;

        public LessonUseCase(ILessonRepository lessonPort)
        {
            _lessonPort = lessonPort;
        }

        public Task<IEnumerable<Lesson>> GetAll() => _lessonPort.GetAllAsync();

        public Task<IEnumerable<Lesson>> GetPublished() => _lessonPort.GetPublishedAsync();

        public Task<Lesson?> GetById(int id) => _lessonPort.GetByIdAsync(id);

        /// <summary>
        /// Crea una lección/señal. La validación de reglas (kind válido, señal con coin +
        /// recommendation) la realiza el constructor de Lesson.
        /// </summary>
        public async Task<Lesson> Create(LessonCommand req, string authorId, string authorName)
        {
            var lesson = new Lesson(
                req.Kind,
                req.Title,
                req.Body,
                authorId,
                authorName,
                req.CoinId,
                req.CoinSymbol,
                req.Recommendation);

            await _lessonPort.CreateAsync(lesson);
            return lesson;
        }

        /// <summary>Actualiza el contenido. Devuelve null si la lección no existe.</summary>
        public async Task<Lesson?> Update(int id, LessonCommand req)
        {
            var lesson = await _lessonPort.GetByIdAsync(id);
            if (lesson is null) return null;

            lesson.UpdateContent(req.Title, req.Body, req.CoinId, req.CoinSymbol, req.Recommendation);
            await _lessonPort.UpdateAsync(lesson);
            return lesson;
        }

        /// <summary>Borra una lección. Devuelve false si no existe.</summary>
        public async Task<bool> Delete(int id)
        {
            var lesson = await _lessonPort.GetByIdAsync(id);
            if (lesson is null) return false;

            await _lessonPort.DeleteAsync(id);
            return true;
        }

        /// <summary>Publica una lección. Devuelve null si no existe.</summary>
        public async Task<Lesson?> Publish(int id)
        {
            var lesson = await _lessonPort.GetByIdAsync(id);
            if (lesson is null) return null;

            lesson.Publish();
            await _lessonPort.UpdateAsync(lesson);
            return lesson;
        }

        /// <summary>Despublica una lección. Devuelve null si no existe.</summary>
        public async Task<Lesson?> Unpublish(int id)
        {
            var lesson = await _lessonPort.GetByIdAsync(id);
            if (lesson is null) return null;

            lesson.Unpublish();
            await _lessonPort.UpdateAsync(lesson);
            return lesson;
        }
    }
}
