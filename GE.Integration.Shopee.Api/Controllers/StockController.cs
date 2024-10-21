using GE.Integration.Shopee.Application.Services.Stock;
using GE.Integration.Shopee.Domain.Response.WebHook;
using Microsoft.AspNetCore.Mvc;

namespace GE.Integration.Shopee.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet("{sellerId}")]
        public async Task<IActionResult> GetStockDetail([FromHeader] string token, int sellerId, DateTime dateCreatedFrom, DateTime dateCreatedTo, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _stockService.GetItemList(token, dateCreatedFrom, dateCreatedTo, sellerId, cancellationToken));

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpPost("Detail")]
        public async Task<IActionResult> GetItemListDetail(WebHookPost webHookMessage, CancellationToken cancellationToken)
        {
            return Ok(await _stockService.GetItemDetail(webHookMessage, cancellationToken));
        }
    }
}
