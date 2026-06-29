using CryptoAppBackEnd.Domains.Entities.Errors;
using CryptoAppBackEnd.Infraestructure.Persistence.Models;

namespace CryptoAppBackEnd.Infraestructure.Persistence.Mappers
{
    public static class AppErrorMapper
    {
        public static AppErrorDbModel ToDb(AppError domain) => new()
        {
            id = domain.id,
            route = domain.route,
            message = domain.message,
            stack = domain.stack,
            user_id = domain.user_id,
            user_email = domain.user_email,
            created_at = domain.created_at
        };

        public static AppError ToDomain(AppErrorDbModel db) => AppError.FromPersistence(
            db.id,
            db.route,
            db.message,
            db.stack,
            db.user_id,
            db.user_email,
            db.created_at);
    }
}
