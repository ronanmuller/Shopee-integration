using System.Text;
using GE.Integration.Shopee.Domain.Response.Auth;
using GE.Integration.Shopee.Domain.Response.Products;
using GE.Integration.Shopee.Infra.External.Handlers;
using Newtonsoft.Json;
using RestSharp;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GE.Integration.Shopee.Infra.External.Auth
{
    public class AuthUserShopeeExternal : IAuthUsersShopeeExternal
    {
        private readonly IAuth _auth;
        private readonly HttpClient _httpClient;

        private static readonly string UrlCallback = Environment.GetEnvironmentVariable("URL_CALLBACK_SHOPEE") ?? "https://app.dev.gestaoecommerce.com.br/autorizado/Shopee";

        private readonly string? _partnerId = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PARTNER_ID"))
            ? throw new Exception("PARTNER_ID - EMPTY Env Variable ****")
            : Environment.GetEnvironmentVariable("PARTNER_ID");

        private readonly string? _hostShopee = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HOST_SHOPEE"))
            ? throw new Exception("HOST_SHOPEE - EMPTY Env Variable ****")
            : Environment.GetEnvironmentVariable("HOST_SHOPEE");

        public AuthUserShopeeExternal(IHttpClientFactory httpClientFactory, IAuth auth)
        {
            _auth = auth;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<string> GetAuthUrl(string externalId)
        {
            try
            {
                var path = "/api/v2/shop/auth_partner";
                var host = _hostShopee;

                var url = _auth.GenerateAuthUrl(host, path, UrlCallback, Convert.ToInt32(_partnerId));
                url = url + $"?externalId={externalId}";
                return url;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception($"[Integration.Shopee][External][Users][GetAuthUrl] API Error : {ex.Message} - ", ex);
            }
        }

        public async Task<AuthUserResponse> GetToken(int shopId, string clientCode, CancellationToken cancellationToken)
        {
            try
            {
                var path = "/api/v2/auth/token/get";
                var host = _hostShopee;

                var timeStamp = _auth.GetCurrentUnixTimestamp();

                if (!_auth.IsTimestampValid(timeStamp))
                    return new AuthUserResponse { error = "Invalid Timestamp" };

                var body = new
                {
                    partner_id = Convert.ToInt32(_partnerId),
                    shop_id = shopId,
                    code = clientCode,
                };

                var jsonBody = JsonConvert.SerializeObject(body);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var url = _auth.GenerateAuthUrl(host, path, string.Empty, Convert.ToInt32(_partnerId));

                var responseContent = await SendAsync(url, HttpMethod.Post, content, cancellationToken);

                if (!string.IsNullOrEmpty(responseContent))
                {
                    var itemListResponse = JsonConvert.DeserializeObject<AuthUserResponse>(responseContent);
                    return itemListResponse ?? throw new Exception("Failed to deserialize response");
                }

                if (string.IsNullOrEmpty(responseContent))
                    Console.Write("Response content is null or empty.");
                
            }
            catch (Exception ex)
            {
                return new AuthUserResponse { error_content = new AuthUserResponseError { Error = ex.Message } };
            }

            return new AuthUserResponse { error_content = new AuthUserResponseError { Error = "Failed to fetch data." } };
        }

        public async Task<AuthUserResponse> GetRefreshToken(int shopId, string refreshToken, CancellationToken cancellationToken)
        {
            try
            {
                var path = "/api/v2/auth/access_token/get";
                var host = _hostShopee;

                var timeStamp = _auth.GetCurrentUnixTimestamp();
                if (!_auth.IsTimestampValid(timeStamp))
                    return new AuthUserResponse { error = "Invalid Timestamp" };

                var sign = _auth.GenerateRefreshTokenSign(Convert.ToInt32(_partnerId), path, timeStamp);

                var body = new
                {
                    partner_id = Convert.ToInt32(_partnerId),
                    shop_id = shopId,
                    refresh_token = refreshToken,
                    timeStamp = timeStamp,
                    sign = sign,

                };

                var url = _auth.GenerateAuthUrl(host, path, string.Empty, Convert.ToInt32(_partnerId));
                var jsonBody = JsonConvert.SerializeObject(body);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var responseContent = await SendAsync(url, HttpMethod.Post, content, cancellationToken);

                if (!string.IsNullOrEmpty(responseContent))
                {
                    var itemListResponse = JsonConvert.DeserializeObject<AuthUserResponse>(responseContent);
                    return itemListResponse ?? throw new Exception("Failed to deserialize response");
                }

                if (string.IsNullOrEmpty(responseContent))
                    Console.WriteLine("Response content is null or empty.");

            }
            catch (Exception ex)
            {
                return new AuthUserResponse { error_content = new AuthUserResponseError { Error = ex.Message } };
            }

            return new AuthUserResponse { error_content = new AuthUserResponseError { Error = "Failed to fetch data." } };
        }


        private async Task<string> SendAsync(string url, HttpMethod method, HttpContent content, CancellationToken cancellationToken)
        {
            try
            {
                var request = new HttpRequestMessage(method, url) { Content = content };
                var response = await _httpClient.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync(cancellationToken);
                }

                throw new HttpRequestException($"[Integration.Shopee][External][Users][SendAsync] API request failed  - Status Code: {response.StatusCode} - RequestUri: {request.RequestUri}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Integration.Shopee][External][Users][SendAsync] API Error : {ex.Message} - RequestUri: {url}", ex);
            }

            return string.Empty;
        }
    }
}
