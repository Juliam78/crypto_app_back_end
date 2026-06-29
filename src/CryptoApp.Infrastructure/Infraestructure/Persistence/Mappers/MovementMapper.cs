using CryptoAppBackEnd.Domains.Entities.Movements;
using CryptoAppBackEnd.Infraestructure.Persistence.Models;

namespace CryptoAppBackEnd.Infraestructure.Persistence.Mappers
{
    public static class MovementMapper
    {
        public static MovementDbModel ToDb(Movement domain) => new()
        {
            id = domain.id,
            person_id = domain.person_id,
            portfolio_id = domain.portfolio_id,
            crypto_id = domain.crypto_id,
            type = domain.type,
            quantity = domain.quantity,
            price = domain.price,
            total = domain.total,
            realized_pnl = domain.realized_pnl,
            created_at = domain.created_at
        };

        public static Movement ToDomain(MovementDbModel db) => Movement.FromPersistence(
            db.id,
            db.person_id,
            db.portfolio_id,
            db.crypto_id,
            db.type,
            db.quantity,
            db.price,
            db.total,
            db.realized_pnl,
            db.created_at);
    }
}
