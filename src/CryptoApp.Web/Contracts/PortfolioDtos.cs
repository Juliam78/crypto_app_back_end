using CryptoAppBackEnd.Domains.Entities.Portfolios;

namespace CryptoApp.Web.Contracts
{
    public record CreatePortfolioRequest(int PersonId, string Name, string BaseCurrency);

    public record UpdatePortfolioRequest(int PersonId, string Name, string BaseCurrency);

    public record PortfolioResponse(
        string Id,
        string PersonId,
        string Name,
        string BaseCurrency,
        string CreatedAt,
        string UpdatedAt)
    {
        public static PortfolioResponse From(Portfolio p) => new(
            p.id.ToString(),
            p.person_id.ToString(),
            p.name,
            p.base_currency,
            p.created_at.ToString("o"),
            p.updated_at.ToString("o"));
    }
}
