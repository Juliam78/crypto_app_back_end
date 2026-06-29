using CryptoAppBackEnd.Domains.Entities.Movements;
using CryptoAppBackEnd.Infraestructure.Persistence.Models;

namespace CryptoAppBackEnd.Infraestructure.Persistence.Mappers
{
    public static class MovementMapper
    {
        public static MovementDbModel ToDb(Movement domain) => new()
        {
            id = domain.id,
            user_id = domain.user_id,
            user_name = domain.user_name,
            coin_id = domain.coin_id,
            coin_name = domain.coin_name,
            coin_symbol = domain.coin_symbol,
            type = domain.type,
            quantity = domain.quantity,
            currency = domain.currency,
            price = domain.price,
            total = domain.total,
            realized_pnl = domain.realized_pnl,
            created_at = domain.created_at
        };

        public static Movement ToDomain(MovementDbModel db) => Movement.FromPersistence(
            db.id,
            db.user_id,
            db.user_name,
            db.coin_id,
            db.coin_name,
            db.coin_symbol,
            db.type,
            db.quantity,
            db.currency,
            db.price,
            db.total,
            db.realized_pnl,
            db.created_at);
    }
}
