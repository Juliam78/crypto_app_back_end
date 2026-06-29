using CryptoAppBackEnd.Domains.Entities.Academy;

namespace CryptoAppBackEnd.Application.Ports
{
    public interface ILessonRepository
    {
        /// <summary>Todas las lecciones/señales (staff), ordenadas por fecha de creación descendente.</summary>
        Task<IEnumerable<Lesson>> GetAllAsync();

        /// <summary>Solo las publicadas, ordenadas por fecha de creación descendente.</summary>
        Task<IEnumerable<Lesson>> GetPublishedAsync();

        /// <summary>Una lección por id, o null si no existe.</summary>
        Task<Lesson?> GetByIdAsync(int id);

        /// <summary>Persiste la lección y devuelve la entidad con el id generado por la BD.</summary>
        Task<Lesson> CreateAsync(Lesson lesson);

        Task UpdateAsync(Lesson lesson);

        Task DeleteAsync(int id);
    }
}
