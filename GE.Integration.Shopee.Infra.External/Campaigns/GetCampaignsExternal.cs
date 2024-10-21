using GE.Integration.Shopee.Domain.Response.Auth;
using GE.Integration.Shopee.Domain.Response.Campaign;
using GE.Integration.Shopee.Infra.External.Handlers;
using Newtonsoft.Json;
using RestSharp;

namespace GE.Integration.Shopee.Infra.External.Campaigns
{
    public class GetCampaignsExternal : IGetCampaignsExternal
    {
        private readonly IAuth _auth;
        private static readonly string PartnerId = Environment.GetEnvironmentVariable("PARTNER_ID") ?? "1143126";
        private static readonly string HostShopee = Environment.GetEnvironmentVariable("HOST_SHOPEE") ?? "https://partner.test-stable.shopeemobile.com";

        public GetCampaignsExternal( IAuth auth)
        {
            _auth = auth;
        }

        public async Task<CampaignResponse> GetItemDetail(Guid customerId, string accessToken, int shopId, DateTime dateCreatedFrom, DateTime dateCreatedTo, CancellationToken cancellationToken)
        {
            AuthUserResponseError? error = null;
            var path = "/api/v2/ads/get_all_cpc_ads_daily_performance";

            try
            {
                var timeStamp = _auth.GetCurrentUnixTimestamp();
                var sign = _auth.GenerateSignShop(timeStamp, path, accessToken, shopId, Convert.ToInt32(PartnerId));
                var url = BuildUrl(path, accessToken, shopId, timeStamp, sign, dateCreatedFrom, dateCreatedTo);
                var client = new RestClient(url);
                var request = new RestRequest();
                var response = await client.ExecuteGetAsync(request, cancellationToken);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var itemListResponse = JsonConvert.DeserializeObject<CampaignResponse>(response.Content);
                    return itemListResponse ?? throw new Exception("Failed to deserialize response");
                }

                if (string.IsNullOrEmpty(response.Content))
                {
                    throw new Exception("Response content is null or empty.");
                }

                Console.WriteLine($"Error: {response.Content}");
                error = JsonConvert.DeserializeObject<AuthUserResponseError>(response.Content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Erro ao buscar campanhas da shopee] - {ex.Message} - {ex.StackTrace}");
            }

            return new CampaignResponse { ErrorContent = error };
        }

        private string BuildUrl(string path, string accessToken, int shopId, long timeStamp, string sign, DateTime dataIni, DateTime dateEnd)
        {
            var url = $"{HostShopee}{path}" +
                      $"?access_token={accessToken}" +
                      $"&partner_id={Convert.ToInt32(PartnerId)}" +
                      $"&shop_id={shopId}" +
                      $"&sign={sign}" +
                      $"&timestamp={timeStamp}";

            return url;
        }   
    }
}
