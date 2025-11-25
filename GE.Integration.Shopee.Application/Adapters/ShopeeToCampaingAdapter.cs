using GE.Integration.Shopee.Application.Helpers;
using Response = GE.Integration.Shopee.Domain.Response.Campaign.Response;

namespace GE.Integration.Shopee.Application.Adapters
{
    public class ShopeeToCampaingAdapter : ICampaignAdapter
    {
        private readonly Response _campaign;

        public ShopeeToCampaingAdapter(Response campaign)
        {
            _campaign = campaign;
        }

        public long CampaignId() => ExtensionsHelper.GenerateUniqueRandomNumber(1, 9999999999); // Gera 10 números únicos com no máximo 10 dígitos

        public string? Name() => string.Empty;

        public string Type() => "custom";

        public string Status() => "active";

        public double Budget()
        {
            return 0;
        }

        public DateTime? LastUpdated() => DateTime.Today;

        public DateTime? DateCreated()
        {
            if (_campaign.Date != null)
            {
                if (DateTime.TryParse(_campaign.Date, out var date))
                    return date;
            }

            return DateTime.Today;
        }

        public long Clicks() => _campaign.Clicks;

        public long Impressions() => _campaign.Impression;

        public double? Ctr() => _campaign.Ctr;

        public double? Cost() => _campaign.Expense;

        public double? Cpc() => 0;

        public long? SoldQuantityDirect() => _campaign.DirectOrder;

        public long? SoldQuantityIndirect() => _campaign.BroadOrder;

        public long? SoldQuantityTotal() => 0;

        public double? AmountDirect() => 0;

        public double? AmountIndirect() => 0;

        public double? AmountTotal() => _campaign.BroadGmv;

        public double? AdvertisingFee() => _campaign.Expense;

        public double? Share() => 0;

        public long? OrganicSoldItemsQuantity() => 0;

        public long? SoldItemsQuantityDirect() => _campaign.DirectItemSold;

        public long? SoldItemsQuantityIndirect() => _campaign.BroadItemSold;

        public long? SoldItemsConversion() => 0;

        public Guid? CustomerId() => Guid.Empty;
    }
}
