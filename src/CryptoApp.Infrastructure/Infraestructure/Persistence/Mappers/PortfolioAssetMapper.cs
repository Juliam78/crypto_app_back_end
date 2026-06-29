using CryptoAppBackEnd.Domains.Entities.Portfolios;
using CryptoAppBackEnd.Infraestructure.Persistence.Models;

namespace CryptoAppBackEnd.Infraestructure.Persistence.Mappers
{
    public static class PortfolioAssetMapper
    {
        public static PortfolioAssetDbModel ToDb(PortfolioAsset domain) => new()
        {
            id = domain.id,
            portfolio_id = domain.portfolio_id,
            crypto_id = domain.crypto_id,
            quantity = domain.quantity,
            average_buy_price = domain.average_buy_price,
            total_invested = domain.total_invested,
            created_at = domain.created_at,
            updated_at = domain.updated_at
        };

        public static PortfolioAsset ToDomain(PortfolioAssetDbModel db) => PortfolioAsset.FromPersistence(
            db.id,
            db.portfolio_id,
            db.crypto_id,
            db.quantity,
            db.average_buy_price,
            db.total_invested,
            db.created_at,
            db.updated_at);
    }
}
