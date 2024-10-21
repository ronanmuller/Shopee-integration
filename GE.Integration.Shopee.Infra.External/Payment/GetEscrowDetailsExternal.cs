using GE.Integration.Shopee.Domain.Response.Auth;
using GE.Integration.Shopee.Domain.Response.Payment;
using GE.Integration.Shopee.Infra.External.Handlers;
using Newtonsoft.Json;
using RestSharp;

namespace GE.Integration.Shopee.Infra.External.Payment
{
    public class GetEscrowDetailsExternal : IGetEscrowDetailsExternal
    {
        private readonly IAuth _auth;

        private readonly string? _partnerId = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PARTNER_ID"))
            ? throw new Exception("PARTNER_ID - EMPTY Env Variable ****")
            : Environment.GetEnvironmentVariable("PARTNER_ID");

        private readonly string? _hostShopee = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HOST_SHOPEE"))
            ? throw new Exception("HOST_SHOPEE - EMPTY Env Variable ****")
            : Environment.GetEnvironmentVariable("HOST_SHOPEE");

        public GetEscrowDetailsExternal(IAuth auth)
        {
            _auth = auth;
        }
        public async Task<EscrowDetailResponse> GetEscrowDetails(string accessToken, int shopId, string orderSn)
        {
            try
            {
                var host = _hostShopee;
                var path = "/api/v2/payment/get_escrow_detail";

                var timeStamp = _auth.GetCurrentUnixTimestamp();
                var sign = _auth.GenerateSignShop(timeStamp, path, accessToken, shopId, Convert.ToInt32(_partnerId));


                var url = $"{host}{path}" +
                          $"?partner_id={Convert.ToInt32(_partnerId)}" +
                          $"&sign={sign}" +
                          $"&timestamp={timeStamp}" +
                          $"&access_token={accessToken}" +
                          $"&shop_id={shopId}" +
                          $"&order_sn={orderSn}";

                var client = new RestClient(url);
                var request = new RestRequest();

                var response = await client.ExecuteGetAsync(request);

                if (response is { IsSuccessful: true, Content: not null })
                {
                    var itemListResponse = JsonConvert.DeserializeObject<EscrowDetailResponse>(response.Content);

                    var errorContent = JsonConvert.DeserializeObject<AuthUserResponseError>(response.Content ?? "");

                    if (errorContent != null && 
                        (!string.IsNullOrEmpty(errorContent.Message) || !string.IsNullOrEmpty(errorContent.Error)))
                    {
                        Console.WriteLine($"Error: {errorContent.Message} -- {errorContent.Error}");

                        return new EscrowDetailResponse()
                        {
                            ErrorContent = errorContent
                        };
                    }

                    return itemListResponse ?? throw new Exception("Failed to deserialize response");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"[Integration.Shopee][Externa][GetEscrowDetails] API Error : {ex.Message} - {ex.StackTrace}");
                throw new Exception(
                    $"[Integration.Shopee][Externa][GetEscrowDetails] API Error : {ex.Message} - {ex.StackTrace}");
            }


            return new EscrowDetailResponse()
            {
                ErrorContent = new()
                {
                    Error = "Erro no método GetEscrowDetails. Response não foi obtido no request."
                }
            };
        }

    }

}
