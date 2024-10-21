using GE.Integration.Shopee.Domain.Response.WebHook;

namespace GE.Integration.Shopee.Application.Services.Campaign
{
    public interface ICampaignService
    {
        Task<string> GetItemList(Guid customerId, string accessToken, int shopId, DateTime dateCreatedFrom,
            DateTime dateCreatedTo, CancellationToken cancellationToken);
    }
}
