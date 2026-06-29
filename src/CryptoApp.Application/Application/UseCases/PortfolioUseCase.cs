using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.Portfolios;

namespace CryptoAppBackEnd.Application.UseCases
{
    public class PortfolioUseCase
    {
        private readonly IPortfolioRepository _portfolioPort;
        public PortfolioUseCase(IPortfolioRepository portfolioPort)
        {
            _portfolioPort = portfolioPort;
        }

        public async Task CreatePortfolio(Portfolio portfolio)
        {
            await _portfolioPort.CreatePortfolioAsync(portfolio);
        }

        public async Task DeletePortfolio(int id)
        {
            await _portfolioPort.DeletePortfolioAsync(id);
        }

        public async Task UpdatePortfolio(Portfolio portfolio)
        {
            await _portfolioPort.UpdatePortfolioAsync(portfolio.id, portfolio);
        }

        public async Task<Portfolio> GetPortfolioById(int id)
        {
            return await _portfolioPort.GetPortfolioAsync(id);
        }

        public async Task<IEnumerable<Portfolio>> GetPortfolios()
        {
            return await _portfolioPort.GetPortfoliosAsync();
        }
    }
}
