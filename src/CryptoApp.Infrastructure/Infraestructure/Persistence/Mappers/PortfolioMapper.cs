using CryptoAppBackEnd.Domains.Entities.Portfolios;
using CryptoAppBackEnd.Infraestructure.Persistence.Models;

namespace CryptoAppBackEnd.Infraestructure.Persistence.Mappers
{
    public static class PortfolioMapper
    {
        public static PortfolioDbModel ToDb(Portfolio domain) => new()
        {
            id = domain.id,
            person_id = domain.person_id,
            name = domain.name,
            base_currency = domain.base_currency,
            created_at = domain.created_at,
            updated_at = domain.updated_at
        };

        public static Portfolio ToDomain(PortfolioDbModel db) => Portfolio.FromPersistence(
            db.id,
            db.person_id,
            db.name,
            db.base_currency,
            db.created_at,
            db.updated_at);
    }
}
