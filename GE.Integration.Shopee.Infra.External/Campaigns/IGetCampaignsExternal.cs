using GE.Integration.Shopee.Domain.Response.Campaign;

namespace GE.Integration.Shopee.Infra.External.Campaigns
{
    public interface IGetCampaignsExternal
    {
        Task<CampaignResponse> GetItemDetail(Guid customerId, string accessToken, int shopId, DateTime dateCreatedFrom, DateTime dateCreatedTo, CancellationToken cancellationToken);
    }
}
