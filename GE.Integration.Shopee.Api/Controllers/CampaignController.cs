 using GE.Integration.Shopee.Application.Services.Campaign;
using Microsoft.AspNetCore.Mvc;

namespace GE.Integration.Shopee.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _campaignService;

        public CampaignController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        [HttpGet("Seller/{sellerId}")]
        public async Task<IActionResult> GetCampaign([FromHeader] string token, int sellerId, DateTime dateCreatedFrom, DateTime dateCreatedTo, Guid customerId, CancellationToken cancellationToken)
        {
            return Ok(await _campaignService.GetItemList(customerId, token, sellerId, dateCreatedFrom, dateCreatedTo, cancellationToken));
        }
    }
}