using GE.Integration.Shopee.Application.Services.Product;
using GE.Integration.Shopee.Domain.Response.WebHook;
using Microsoft.AspNetCore.Mvc;

namespace GE.Integration.Shopee.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("Seller/{sellerId}")]
        public async Task<IActionResult> Get([FromHeader] string token, int sellerId, DateTime dateCreatedFrom, DateTime dateCreatedTo, 
            long? customerPlanId, Guid customerId, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _productService.GetItemList(customerId, token, sellerId,  dateCreatedFrom, dateCreatedTo, customerPlanId, cancellationToken));

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

      
        [HttpPost("Detail")]
        public async Task<IActionResult> GetItemListDetail(WebHookPost productWebHookMessage, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _productService.GetItemDetail(productWebHookMessage));

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }
    }
}
