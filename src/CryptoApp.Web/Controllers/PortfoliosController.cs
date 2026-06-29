using CryptoApp.Web.Contracts;
using CryptoAppBackEnd.Application.UseCases;
using CryptoAppBackEnd.Domains.Entities.Portfolios;
using Microsoft.AspNetCore.Mvc;

namespace CryptoApp.Web.Controllers
{
    [ApiController]
    [Route("api/portfolios")]
    public class PortfoliosController : ControllerBase
    {
        private readonly PortfolioUseCase _useCase;

        public PortfoliosController(PortfolioUseCase useCase)
        {
            _useCase = useCase;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PortfolioResponse>>> GetAll()
        {
            var portfolios = await _useCase.GetPortfolios();
            return Ok(portfolios.Select(PortfolioResponse.From));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PortfolioResponse>> GetById(int id)
        {
            var portfolio = await _useCase.GetPortfolioById(id);
            if (portfolio is null)
            {
                return NotFound(new { message = $"No existe un portafolio con id {id}." });
            }

            return Ok(PortfolioResponse.From(portfolio));
        }

        [HttpPost]
        public async Task<ActionResult<PortfolioResponse>> Create([FromBody] CreatePortfolioRequest request)
        {
            try
            {
                var portfolio = new Portfolio(request.Name, request.BaseCurrency);
                portfolio.AssignToPerson(request.PersonId);
                await _useCase.CreatePortfolio(portfolio);
                return CreatedAtAction(nameof(GetById), new { id = portfolio.id }, PortfolioResponse.From(portfolio));
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<PortfolioResponse>> Update(int id, [FromBody] UpdatePortfolioRequest request)
        {
            var existing = await _useCase.GetPortfolioById(id);
            if (existing is null)
            {
                return NotFound(new { message = $"No existe un portafolio con id {id}." });
            }

            try
            {
                existing.UpdateDetails(request.PersonId, request.Name, request.BaseCurrency);
                await _useCase.UpdatePortfolio(existing);
                return Ok(PortfolioResponse.From(existing));
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
                await _useCase.DeletePortfolio(id);
                return NoContent();
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
