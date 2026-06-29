using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.Portfolios;

namespace CryptoAppBackEnd.Application.UseCases
{
    public class PortfolioAssetUseCase
    {
        private readonly IPortfolioAssetRepository _portfolioAssetPort;

        public PortfolioAssetUseCase(IPortfolioAssetRepository portfolioAssetPort)
        {
            _portfolioAssetPort = portfolioAssetPort;
        }

        public async Task CreatePortfolioAsset(PortfolioAsset porfolioAsset)
        {
            await _portfolioAssetPort.CreatePortfolioAsset(porfolioAsset);
        }

        public async Task DeletePorfolioAsset(int id)
        {
            await _portfolioAssetPort.DeletePortfolioAssetAsync(id);
        }

        public async Task UpdatePorfolioAsset(PortfolioAsset portfolioAsset)
        {
            await _portfolioAssetPort.UpdatePortfolioAssetAsync(portfolioAsset.id, portfolioAsset);
        }

        public async Task<PortfolioAsset> GetPorfolioAssetById(int id)
        {
            return await _portfolioAssetPort.GetPortfolioAssetAsync(id);
        }

        public async Task<IEnumerable<PortfolioAsset>> GetPorfolioAssets()
        {
            return await _portfolioAssetPort.GetPortfolioAssetsAsync();
        }
    }
}
