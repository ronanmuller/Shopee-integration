using GE.Integration.Shopee.Domain.Response.Auth;
using GE.Integration.Shopee.Domain.Response.Shop;
using GE.Integration.Shopee.Infra.External.Handlers;
using Newtonsoft.Json;
using RestSharp;

namespace GE.Integration.Shopee.Infra.External.Shop
{
    public class GetShopInfoExternal : IGetShopInfoExternal
    {
        private readonly IAuth _auth;

        private readonly string? _partnerId = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PARTNER_ID"))
            ? throw new Exception("PARTNER_ID - EMPTY Env Variable ****")
            : Environment.GetEnvironmentVariable("PARTNER_ID");

        private readonly string? _hostShopee = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HOST_SHOPEE"))
            ? throw new Exception("HOST_SHOPEE - EMPTY Env Variable ****")
            : Environment.GetEnvironmentVariable("HOST_SHOPEE");

        public GetShopInfoExternal(IAuth auth)
        {
            _auth = auth;
        }

        private string BuildUrl(string accessToken, int shopId, long timeStamp, string sign)
        {
            return $"{_hostShopee}/api/v2/shop/get_shop_info" +
                   $"?access_token={accessToken}" +
                   $"&partner_id={Convert.ToInt32(_partnerId)}" +
                   $"&timestamp={timeStamp}" +
                   $"&shop_id={shopId}" +
                   $"&sign={sign}";
        }

        public async Task<ShopInfoResponse> GetShopInfoDetails(string accessToken, int shopId)
        {
            try
            {
                var timeStamp = _auth.GetCurrentUnixTimestamp();
                var path = "/api/v2/shop/get_shop_info";
                var sign = _auth.GenerateSignShop(timeStamp, path, accessToken, shopId, Convert.ToInt32(_partnerId));
                var url = BuildUrl(accessToken, shopId, timeStamp, sign);

                var client = new RestClient(url);
                var request = new RestRequest();

                var response = await client.ExecuteGetAsync(request);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var itemListResponse = JsonConvert.DeserializeObject<ShopInfoResponse>(response.Content);

                    if (itemListResponse == null)
                    {
                        // Se itemListResponse for nulo após a desserialização, retornar um erro.
                        throw new Exception("Failed to deserialize response: itemListResponse is null");
                    }

                    if (!string.IsNullOrEmpty(itemListResponse.error))
                    {
                        var error = JsonConvert.DeserializeObject<AuthUserResponseError>(response.Content);
                        return new ShopInfoResponse()
                        {
                            ErrorContent = error
                        };
                    }

                    return itemListResponse;
                }

                var statusCode = response?.StatusCode;
                Console.WriteLine($"Failed to fetch data. Status code: {statusCode}");
                throw new Exception($"Failed to fetch data. Status code: {statusCode}");
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"Failed to deserialize JSON response: {jsonEx.Message}");
                throw new Exception($"Failed to deserialize JSON response: {jsonEx.Message}", jsonEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                throw new Exception($"API Error: {ex.Message}", ex);
            }
        }
    }
}
