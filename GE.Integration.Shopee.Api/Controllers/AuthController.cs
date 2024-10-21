using GE.Integration.Shopee.Application.Services.Auth;
using GE.Integration.Shopee.Application.Services.Order;
using GE.Integration.Shopee.Infra.External.Auth;
using Microsoft.AspNetCore.Mvc;

namespace GE.Integration.Shopee.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthUsersShopeeExternal _authUsersExternal;
        private readonly IAuthService _authService;
        private readonly IOrderService _orderService;

        public AuthController(IAuthUsersShopeeExternal authUsersExternal, IAuthService authService, IOrderService orderService)
        {
            _authUsersExternal = authUsersExternal;
            _authService = authService;
            _orderService = orderService;
        }

        [HttpGet("Url")]
        public async Task<OkObjectResult> GetUrlAuth([FromQuery] string externalId)
        {
            return Ok(await _authUsersExternal.GetAuthUrl(externalId));
        }

        [HttpGet("Token")]
        public async Task<IActionResult> GetUserToken([FromQuery] string sellerId, [FromQuery] string token, CancellationToken cancellationToken)
        {
            var ret = await _authService.GetToken(Convert.ToInt32(sellerId), token, cancellationToken);

            if (ret.error_content != null)
                return BadRequest(ret.error);

            return Ok(ret);
        }


        [HttpPost("RefreshToken")]
        public async Task<IActionResult> GetUserRefreshToken([FromQuery] string sellerId, [FromQuery] string refreshToken, CancellationToken cancellationToken)
        {
            var ret = await _authUsersExternal.GetRefreshToken(Convert.ToInt32(sellerId), refreshToken, cancellationToken);

            if (ret.error_content != null)
                return BadRequest(ret.error);

            return Ok(ret);
        }

        [HttpGet("ShopInfo")]
        public async Task<IActionResult> GetShopDetail([FromQuery] string accessToken, [FromQuery] string sellerId)
        {
            return Ok(await _orderService.GetShopDetail(accessToken, Convert.ToInt32(sellerId)));
        }
    }
}
