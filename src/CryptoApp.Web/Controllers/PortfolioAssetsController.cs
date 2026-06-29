using CryptoApp.Web.Contracts;
using CryptoAppBackEnd.Application.UseCases;
using CryptoAppBackEnd.Domains.Entities.Portfolios;
using Microsoft.AspNetCore.Mvc;

namespace CryptoApp.Web.Controllers
{
    [ApiController]
    [Route("api/portfolio-assets")]
    public class PortfolioAssetsController : ControllerBase
    {
        private readonly PortfolioAssetUseCase _useCase;

        public PortfolioAssetsController(PortfolioAssetUseCase useCase)
        {
            _useCase = useCase;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PortfolioAssetResponse>>> GetAll()
        {
            var assets = await _useCase.GetPorfolioAssets();
            return Ok(assets.Select(PortfolioAssetResponse.From));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PortfolioAssetResponse>> GetById(int id)
        {
            var asset = await _useCase.GetPorfolioAssetById(id);
            if (asset is null)
            {
                return NotFound(new { message = $"No existe un activo de portafolio con id {id}." });
            }

            return Ok(PortfolioAssetResponse.From(asset));
        }

        [HttpPost]
        public async Task<ActionResult<PortfolioAssetResponse>> Create([FromBody] CreatePortfolioAssetRequest request)
        {
            try
            {
                var asset = new PortfolioAsset(request.Quantity, request.AverageBuyPrice, request.TotalInvested);
                asset.AssignTo(request.PortfolioId, request.CryptoId);
                await _useCase.CreatePortfolioAsset(asset);
                return CreatedAtAction(nameof(GetById), new { id = asset.id }, PortfolioAssetResponse.From(asset));
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<PortfolioAssetResponse>> Update(int id, [FromBody] UpdatePortfolioAssetRequest request)
        {
            var existing = await _useCase.GetPorfolioAssetById(id);
            if (existing is null)
            {
                return NotFound(new { message = $"No existe un activo de portafolio con id {id}." });
            }

            try
            {
                existing.UpdateHolding(
                    request.PortfolioId,
                    request.CryptoId,
                    request.Quantity,
                    request.AverageBuyPrice,
                    request.TotalInvested);
                await _useCase.UpdatePorfolioAsset(existing);
                return Ok(PortfolioAssetResponse.From(existing));
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _useCase.DeletePorfolioAsset(id);
                return NoContent();
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
