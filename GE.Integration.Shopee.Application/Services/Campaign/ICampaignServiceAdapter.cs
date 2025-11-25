using GE.Integration.Shopee.Application.Adapters;

namespace GE.Integration.Shopee.Application.Services.Campaign
{
    public interface ICampaignServiceAdapter
    {
        Task ConvertToCampaignAdapter(ShopeeToCampaingAdapter returnCampaign);
    }
}