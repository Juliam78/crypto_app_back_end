using CryptoAppBackEnd.Domains.Entities.Academy;
using CryptoAppBackEnd.Infraestructure.Persistence.Models;

namespace CryptoAppBackEnd.Infraestructure.Persistence.Mappers
{
    public static class LessonMapper
    {
        public static LessonDbModel ToDb(Lesson domain) => new()
        {
            id = domain.id,
            kind = domain.kind,
            title = domain.title,
            body = domain.body,
            coin_id = domain.coin_id,
            coin_symbol = domain.coin_symbol,
            recommendation = domain.recommendation,
            author_id = domain.author_id,
            author_name = domain.author_name,
            published = domain.published,
            created_at = domain.created_at,
            updated_at = domain.updated_at
        };

        public static Lesson ToDomain(LessonDbModel db) => Lesson.FromPersistence(
            db.id,
            db.kind,
            db.title,
            db.body,
            db.author_id,
            db.author_name,
            db.published,
            db.created_at,
            db.updated_at,
            db.coin_id,
            db.coin_symbol,
            db.recommendation);
    }
}
