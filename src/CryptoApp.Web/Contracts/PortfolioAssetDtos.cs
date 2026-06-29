using CryptoAppBackEnd.Domains.Entities.Portfolios;

namespace CryptoApp.Web.Contracts
{
    public record CreatePortfolioAssetRequest(
        int PortfolioId,
        int CryptoId,
        decimal Quantity,
        decimal AverageBuyPrice,
        decimal TotalInvested);

    public record UpdatePortfolioAssetRequest(
        int PortfolioId,
        int CryptoId,
        decimal Quantity,
        decimal AverageBuyPrice,
        decimal TotalInvested);

    public record PortfolioAssetResponse(
        string Id,
        string PortfolioId,
        string CryptoId,
        decimal Quantity,
        decimal AverageBuyPrice,
        decimal TotalInvested,
        string CreatedAt,
        string UpdatedAt)
    {
        public static PortfolioAssetResponse From(PortfolioAsset a) => new(
            a.id.ToString(),
            a.portfolio_id.ToString(),
            a.crypto_id.ToString(),
            a.quantity,
            a.average_buy_price,
            a.total_invested,
            a.created_at.ToString("o"),
            a.updated_at.ToString("o"));
    }
}
