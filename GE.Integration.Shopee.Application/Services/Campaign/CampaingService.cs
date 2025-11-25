using GE.Integration.Shopee.Application.Adapters;
using GE.Integration.Shopee.Application.Aws;
using GE.Integration.Shopee.Infra.External.Campaigns;
using Newtonsoft.Json;
using GE.Integration.Shopee.Infra.External.Products;
using GE.Integration.Shopee.Domain.Enum;
using GE.Integration.Shopee.Domain.Request;
using static System.Formats.Asn1.AsnWriter;

namespace GE.Integration.Shopee.Application.Services.Campaign
{
    public class CampaignService : ICampaignService
    {
        private readonly IGetCampaignsExternal _campaignExternalService;
        private readonly IAwsService _awsService;
        private readonly ICampaignServiceAdapter _campaignServiceAdapter;
        private const string SHOPEE = "SHOPEE";


        private readonly string AWS_CAMPAIGN_INTEGRATION_CORE = Environment.GetEnvironmentVariable("AWS_CAMPAIGN_INTEGRATION_CORE");

        public CampaignService(
            IGetCampaignsExternal externalService,
            IAwsService awsService,
            ICampaignServiceAdapter campaignServiceAdapter)
        {
            _campaignExternalService = externalService;
            _awsService = awsService;
            _campaignServiceAdapter = campaignServiceAdapter;
        }

        public async Task<string> GetItemList(Guid customerId, string accessToken, int shopId, DateTime dateCreatedFrom, DateTime dateCreatedTo, CancellationToken cancellationToken)
        {
            try
            {
                var campaignResponse = await _campaignExternalService.GetItemDetail(customerId, accessToken, shopId, dateCreatedFrom, dateCreatedTo, cancellationToken);

                var campaignList = campaignResponse?.Response;

                if (campaignList != null)
                {
                    foreach (var campaign in campaignList)
                    {
                        var returnCampaign = new ShopeeToCampaingAdapter(campaign);
                        ////var campaignAdapter = await _campaignServiceAdapter.ConvertToCampaignAdapter(returnCampaign);
                        //campaignAdapter.CustomerId = customerId;

                        //var objectRequest = new ObjectRequest
                        //{
                        //    EntityType = ETypeEntity.Campaign,
                        //    JsonObject = JsonConvert.SerializeObject(campaignAdapter),
                        //    HashIntegracao = SHOPEE
                        //};

                        var json = JsonConvert.SerializeObject(null);

                        await _awsService.SendAwsMessage(json, AWS_CAMPAIGN_INTEGRATION_CORE);
                    }
                }

                return "OK";
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }
        }
    }
}




