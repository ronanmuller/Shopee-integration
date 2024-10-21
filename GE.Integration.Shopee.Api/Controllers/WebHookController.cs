using GE.Integration.Shopee.Application.Services.Order;
using GE.Integration.Shopee.Domain.Response.Orders;
using Microsoft.AspNetCore.Mvc;

namespace GE.Integration.Shopee.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebHookController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public WebHookController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("OrderWebhook")]
        public async Task<IActionResult> PostOrder([FromBody] OrderWebHookShopeePost request, CancellationToken cancellationToken)
        {
            //await _orderService.PostWebhookShopee(request, cancellationToken);

            return Ok();
        }
    }
}
