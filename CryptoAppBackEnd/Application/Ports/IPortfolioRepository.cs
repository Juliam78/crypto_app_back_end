using CryptoAppBackEnd.Domains.Entities.Portfolios;

namespace CryptoAppBackEnd.Application.Ports
{
    public interface IPortfolioRepository
    {
        Task<IEnumerable<Portfolio>> GetPortfoliosAsync();
        Task<Portfolio> GetPortfolioAsync(int id);
        Task CreatePortfolioAsync(Portfolio portfolio);
        Task UpdatePortfolioAsync(int id, Portfolio portfolio);
        Task DeletePortfolioAsync(int id);
    }
}
