using GE.Integration.Shopee.Application.Services.Order;
using GE.Integration.Shopee.Domain.Response.WebHook;
using Microsoft.AspNetCore.Mvc;

namespace GE.Integration.Shopee.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Recebe do Hangfire disparado pelo Core
        /// </summary>
        /// <param name="token"></param>
        /// <param name="orderId"></param>
        /// <param name="dateCreatedFrom"></param>
        /// <param name="dateCreatedTo"></param>
        /// <param name="customerId"></param>
        /// <param name="customerPlanId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("Seller/{orderId}")]
        public async Task<IActionResult> Get([FromHeader] string token, int orderId, DateTime dateCreatedFrom, DateTime dateCreatedTo, Guid customerId, 
            long? customerPlanId, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _orderService.GetOrderList(customerId, customerPlanId, token, orderId, dateCreatedFrom, dateCreatedTo, cancellationToken));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Recebe o response do webhook da fila AWS_SHOPEE_ORDER_DETAILS para enviar pra fila AWS_SHOPEE_ORDER_INTEGRATION_CORE
        /// </summary>
        /// <param name="response"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("Detail")]
        public async Task<IActionResult> GetItemDetails(WebHookPost response, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _orderService.GetOrderDetail(response, cancellationToken));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        /// <summary>
        /// /// <summary>
        /// Deleta os items salvos locais que sao maiores que os dias informados como parametro.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// </summary>
        /// <param name="days">Dias considerados para deletar.</param>
        /// <returns></returns>
        [HttpPost("DeleteOrderStatusRecordsByDays")]
        public async Task<IActionResult> DeleteOrderStatusRecordsByDays(int days = 3)
        {
            try
            {
                return Ok(await _orderService.DeleteLocalBiggerThanXDays(days));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
