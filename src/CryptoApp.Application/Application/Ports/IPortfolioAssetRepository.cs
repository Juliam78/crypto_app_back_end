using CryptoAppBackEnd.Domains.Entities.Portfolios;

namespace CryptoAppBackEnd.Application.Ports
{
    public interface IPortfolioAssetRepository
    {
        Task<IEnumerable<PortfolioAsset>> GetPortfolioAssetsAsync();
        Task<PortfolioAsset> GetPortfolioAssetAsync(int id);
        Task CreatePortfolioAsset(PortfolioAsset portfolioAsset);
        Task UpdatePortfolioAssetAsync(int id, PortfolioAsset portfolioAsset);
        Task DeletePortfolioAssetAsync(int id);
    }
}
