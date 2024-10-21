using Newtonsoft.Json;
using GE.Integration.Shopee.Domain.Response.Auth;
using GE.Integration.Shopee.Domain.Response.Products;
using RestSharp;
using GE.Integration.Shopee.Infra.External.Handlers;

namespace GE.Integration.Shopee.Infra.External.Products
{
    public class GetProductsExternal : IGetProductsExternal
    {
        private readonly IAuth _auth;

        private readonly string? _partnerId = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PARTNER_ID"))
            ? throw new Exception("PARTNER_ID - EMPTY Env Variable ****")
            : Environment.GetEnvironmentVariable("PARTNER_ID");

        private readonly string? _hostShopee = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HOST_SHOPEE"))
            ? throw new Exception("HOST_SHOPEE - EMPTY Env Variable ****")
            : Environment.GetEnvironmentVariable("HOST_SHOPEE");

        public GetProductsExternal(IAuth auth)
        {
            _auth = auth;
        }

        public async Task<ProductListResponse> GetItemList(string accessToken, int shopId, DateTime dateCreatedFrom, DateTime dateCreatedTo, CancellationToken cancellationToken, int page = 0, int pageSize = 100)
        {
            try
            {
                var timeStamp = _auth.GetCurrentUnixTimestamp();
                var sign = _auth.GenerateSignShop(timeStamp, "/api/v2/product/get_item_list", accessToken, shopId, Convert.ToInt32(_partnerId));
                var url = BuildUrl("/api/v2/product/get_item_list", accessToken, shopId, timeStamp, sign, pageSize, page, dateCreatedFrom, dateCreatedTo);

                var client = new RestClient(url);
                var request = new RestRequest();
                var response = await client.ExecuteGetAsync(request, cancellationToken);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var itemListResponse = JsonConvert.DeserializeObject<ProductListResponse>(response.Content);

                    return itemListResponse ?? throw new Exception("Failed to deserialize response");
                }

                if (string.IsNullOrEmpty(response.Content))
                {
                    throw new Exception("Response content is null or empty.");
                }

                HandleErrorResponse(response.Content);
            }
            catch (Exception ex)
            {
                HandleException(ex, "[Integration.Shopee][Externa][GetItemList] API Error");
            }

            return new ProductListResponse { ErrorContent = new AuthUserResponseError { Error = "Failed to fetch data." } };
        }

        public async Task<ProductListDetailResponse> GetItemDetail(string accessToken, int shopId, long itemId, CancellationToken cancellationToken)
        {
            try
            {
                var timeStamp = _auth.GetCurrentUnixTimestamp();
                var sign = _auth.GenerateSignShop(timeStamp, "/api/v2/product/get_item_base_info", accessToken, shopId, Convert.ToInt32(_partnerId));
                var url = BuildUrl("/api/v2/product/get_item_base_info", accessToken, shopId, timeStamp, sign, itemId: itemId);

                var client = new RestClient(url);
                var request = new RestRequest();
                var response = await client.ExecuteGetAsync(request, cancellationToken);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var itemListResponse = JsonConvert.DeserializeObject<ProductListDetailResponse>(response.Content);

                    return itemListResponse ?? throw new Exception("Failed to deserialize response");
                }

                if (string.IsNullOrEmpty(response.Content))
                {
                    throw new Exception("Response content is null or empty.");
                }

                HandleErrorResponse(response.Content);
            }
            catch (Exception ex)
            {
                HandleException(ex, "[Integration.Shopee][Externa][GetItemDetail] API Error");
            }

            return new ProductListDetailResponse
            { ErrorContent = new AuthUserResponseError { Error = "Failed to fetch data." } };
        }

        public async Task<ProductModelListResponse> GetModelItemList(string accessToken, int shopId, long itemId, CancellationToken cancellationToken)
        {
            try
            {
                var path = "/api/v2/product/get_model_list";
                var timeStamp = _auth.GetCurrentUnixTimestamp();
                var sign = _auth.GenerateSignShop(timeStamp, "/api/v2/product/get_model_list", accessToken, shopId, Convert.ToInt32(_partnerId));

                var url = $"{_hostShopee}{path}" +
                          $"?item_id={itemId}" +
                          $"&partner_id={Convert.ToInt32(_partnerId)}" +
                          $"&access_token={accessToken}" +
                          $"&timestamp={timeStamp}" +
                          $"&sign={sign}" +
                          $"&shop_id={shopId}";

                var client = new RestClient(url);
                var request = new RestRequest();
                var response = await client.ExecuteGetAsync(request, cancellationToken);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var itemListResponse = JsonConvert.DeserializeObject<ProductModelListResponse>(response.Content);

                    return itemListResponse ?? throw new Exception("Failed to deserialize response");
                }

                if (string.IsNullOrEmpty(response.Content))
                {
                    throw new Exception("Response content is null or empty.");
                }

                HandleErrorResponse(response.Content);
            }
            catch (Exception ex)
            {
                HandleException(ex, "[Integration.Shopee][Externa][GetItemDetail] API Error");
            }

            return new ProductModelListResponse{ ErrorContent = new AuthUserResponseError { Error = "Failed to fetch data." } };
        }

        public async Task<ProductCategoryResponse> GetCategory(string accessToken, int shopId, CancellationToken cancellationToken)
        {
            try
            {
                var timeStamp = _auth.GetCurrentUnixTimestamp();
                var sign = _auth.GenerateSignShop(timeStamp, "/api/v2/product/get_category", accessToken, shopId, Convert.ToInt32(_partnerId));
                var url = BuildUrl("/api/v2/product/get_category", accessToken, shopId, timeStamp, sign);

                var client = new RestClient(url);
                var request = new RestRequest();
                var response = await client.ExecuteGetAsync(request, cancellationToken);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var itemListResponse = JsonConvert.DeserializeObject<ProductCategoryResponse>(response.Content);
                    return itemListResponse ?? throw new Exception("Failed to deserialize response");
                }

                if (string.IsNullOrEmpty(response.Content))
                {
                    throw new Exception("Response content is null or empty.");
                }

                HandleErrorResponse(response.Content);
            }
            catch (Exception ex)
            {
                HandleException(ex, "[Integration.Shopee][Externa][GetCategory] API Error");
            }

            return new ProductCategoryResponse { ErrorContent = new AuthUserResponseError { Error = "Failed to fetch data." } };
        }

        public async Task<ProductExtraInfoResponse> GetItemExtraInfo(string accessToken, int shopId, CancellationToken cancellationToken, long itemId)
        {
            try
            {
                var timeStamp = _auth.GetCurrentUnixTimestamp();
                var sign = _auth.GenerateSignShop(timeStamp, "/api/v2/product/get_item_extra_info", accessToken, shopId, Convert.ToInt32(_partnerId));
                var url = BuildUrl("/api/v2/product/get_item_extra_info", accessToken, shopId, timeStamp, sign, itemId: itemId);

                var client = new RestClient(url);
                var request = new RestRequest();
                var response = await client.ExecuteGetAsync(request, cancellationToken);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var itemListResponse = JsonConvert.DeserializeObject<ProductExtraInfoResponse>(response.Content);
                    return itemListResponse ?? throw new Exception("Failed to deserialize response");
                }

                if (string.IsNullOrEmpty(response.Content))
                {
                    throw new Exception("Response content is null or empty.");
                }

                HandleErrorResponse(response.Content);
            }
            catch (Exception ex)
            {
                HandleException(ex, "[Integration.Shopee][Externa][GetItemExtraInfo] API Error");
            }

            return new ProductExtraInfoResponse { ErrorContent = new AuthUserResponseError { Error = "Failed to fetch data." } };
        }

        private string BuildUrl(string path, string accessToken, int shopId, long timeStamp, string sign, int pageSize = 100, int page = 0, DateTime? dateCreatedFrom = null, DateTime? dateCreatedTo = null, long? itemId = null)
        {
            var url = $"{_hostShopee}{path}" +
                      $"?page_size={pageSize}" +
                      $"&item_status=NORMAL" +
                      $"&offset={page}" +
                      $"&partner_id={Convert.ToInt32(_partnerId)}" +
                      $"&access_token={accessToken}" +
                      $"&timestamp={timeStamp}" +
                      $"&sign={sign}" +
                      $"&shop_id={shopId}";

            if (itemId.HasValue)
                url += $"&item_id_list={itemId}";

            if (dateCreatedFrom.HasValue && dateCreatedTo.HasValue)
            {
                var initDateTimeOffset = new DateTimeOffset(dateCreatedFrom.Value.Year, dateCreatedFrom.Value.Month, dateCreatedFrom.Value.Day, 0, 0, 0, TimeSpan.Zero);
                var endDateTimeOffset = new DateTimeOffset(dateCreatedTo.Value.Year, dateCreatedTo.Value.Month, dateCreatedTo.Value.Day, 0, 0, 0, TimeSpan.Zero);

                var initTimestamp = initDateTimeOffset.ToUnixTimeSeconds();
                var endTimestamp = endDateTimeOffset.ToUnixTimeSeconds();

                url += $"&update_time_from={initTimestamp}" +
                       $"&update_time_to={endTimestamp}";
            }

            return url;
        }

        private void HandleErrorResponse(string content)
        {
            Console.WriteLine($"Error: {content}");
            var error = JsonConvert.DeserializeObject<AuthUserResponseError>(content);
        }

        private void HandleException(Exception ex, string errorMessage)
        {
            Console.WriteLine($"[{errorMessage}] {ex.Message} - {ex.StackTrace}");
        }

    }
}

