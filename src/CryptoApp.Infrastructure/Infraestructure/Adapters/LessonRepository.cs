using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.Academy;
using CryptoAppBackEnd.Infraestructure.Persistence;
using CryptoAppBackEnd.Infraestructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace CryptoAppBackEnd.Infraestructure.Adapters
{
    public class LessonRepository : ILessonRepository
    {
        private readonly AppDbContext _context;

        public LessonRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Lesson>> GetAllAsync()
        {
            var rows = await _context.Lessons.AsNoTracking()
                .OrderByDescending(l => l.created_at)
                .ToListAsync();
            return rows.Select(LessonMapper.ToDomain).ToList();
        }

        public async Task<IEnumerable<Lesson>> GetPublishedAsync()
        {
            var rows = await _context.Lessons.AsNoTracking()
                .Where(l => l.published)
                .OrderByDescending(l => l.created_at)
                .ToListAsync();
            return rows.Select(LessonMapper.ToDomain).ToList();
        }

        public async Task<Lesson?> GetByIdAsync(int id)
        {
            var row = await _context.Lessons.AsNoTracking()
                .FirstOrDefaultAsync(l => l.id == id);
            return row is null ? null : LessonMapper.ToDomain(row);
        }

        public async Task<Lesson> CreateAsync(Lesson lesson)
        {
            var db = LessonMapper.ToDb(lesson);
            await _context.Lessons.AddAsync(db);
            await _context.SaveChangesAsync();
            // db.id ya viene poblado por la BD; rehidratamos para devolver el id real.
            return LessonMapper.ToDomain(db);
        }

        public async Task UpdateAsync(Lesson lesson)
        {
            var existing = await _context.Lessons.FindAsync(lesson.id);
            if (existing is null)
            {
                return;
            }

            existing.kind = lesson.kind;
            existing.title = lesson.title;
            existing.body = lesson.body;
            existing.coin_id = lesson.coin_id;
            existing.coin_symbol = lesson.coin_symbol;
            existing.recommendation = lesson.recommendation;
            existing.author_id = lesson.author_id;
            existing.author_name = lesson.author_name;
            existing.published = lesson.published;
            existing.updated_at = lesson.updated_at;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.Lessons.FindAsync(id);
            if (existing is null)
            {
                return;
            }

            _context.Lessons.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}
