using GE.Integration.Shopee.Domain.Response.Auth;
using GE.Integration.Shopee.Domain.Response.Orders;
using GE.Integration.Shopee.Infra.External.Handlers;
using Newtonsoft.Json;
using RestSharp;

namespace GE.Integration.Shopee.Infra.External.Orders
{
    public class GetOrdersExternal : IGetOrdersExternal
    {
        private readonly IAuth _auth;

        private readonly string? _partnerId = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PARTNER_ID"))
            ? throw new Exception("PARTNER_ID - EMPTY Env Variable ****")
            : Environment.GetEnvironmentVariable("PARTNER_ID");

        private readonly string? _hostShopee = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HOST_SHOPEE"))
            ? throw new Exception("HOST_SHOPEE - EMPTY Env Variable ****")
            : Environment.GetEnvironmentVariable("HOST_SHOPEE");

        public GetOrdersExternal(IAuth auth)
        {
            _auth = auth;
        }

        public async Task<OrderListResponse> GetOrderList(string accessToken, int shopId, DateTime dateCreatedFrom, DateTime dateCreatedTo, CancellationToken cancellationToken, string cursor, int pageSize)
        {
            try
            {
                var host = _hostShopee;
                var path = "/api/v2/order/get_order_list";

                var timeStamp = _auth.GetCurrentUnixTimestamp();
                var sign = _auth.GenerateSignShop(timeStamp, path, accessToken, shopId, Convert.ToInt32(_partnerId));

                var initDateTimeOffset = new DateTimeOffset(dateCreatedFrom.Year, dateCreatedFrom.Month, dateCreatedFrom.Day, 0, 0, 0, TimeSpan.Zero);
                var endDateTimeOffset = new DateTimeOffset(dateCreatedTo.Year, dateCreatedTo.Month, dateCreatedTo.Day, 0, 0, 0, TimeSpan.Zero);

                var initTimestamp = initDateTimeOffset.ToUnixTimeSeconds();
                var endTimestamp = endDateTimeOffset.ToUnixTimeSeconds();

                var timeRangeField = "create_time";

                var url = $"{host}{path}" +
                          $"?page_size={pageSize}" +
                          $"&timestamp={timeStamp}" +
                          $"&shop_id={shopId}" +
                          $"&partner_id={Convert.ToInt32(_partnerId)}" +
                          $"&access_token={accessToken}" +
                          $"&cursor={cursor}" +
                          $"&time_range_field={timeRangeField}" +
                          $"&time_from={initTimestamp}" +
                          $"&time_to={endTimestamp}" +
                          $"&sign={sign}";

                var client = new RestClient(url);
                var request = new RestRequest();

                var response = await client.ExecuteGetAsync(request, cancellationToken);

                if (response is { IsSuccessful: true } && response.Content != null)
                {
                    var itemListResponse = JsonConvert.DeserializeObject<OrderListResponse>(response.Content);

                    if (itemListResponse != null && !string.IsNullOrEmpty(itemListResponse.error) )
                    {
                        var error = JsonConvert.DeserializeObject<AuthUserResponseError>(response.Content);
                        Console.WriteLine($"Error: {response.Content}");

                        return new OrderListResponse()
                        {
                            ErrorContent = error
                        };
                    }

                    return itemListResponse ?? throw new Exception("Failed to deserialize response");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Integration.Shopee][External][GetOrderList] API Error : {ex.Message} - {ex.StackTrace}");
                throw new Exception($"[Integration.Shopee][External][GetOrderList] API Error : {ex.Message} - {ex.StackTrace}");
            }

            return new OrderListResponse()
            {
                ErrorContent = new()
                {
                    Error = "Erro no método GetItemList. Response não foi obtido no request."
                }
            };
        }

        public async Task<OrderDetailListResponse> GetOrderDetailList(string accessToken, int shopId, string orderSnList, CancellationToken cancellationToken)
        {
            try
            {
                var host = _hostShopee;
                var path = "/api/v2/order/get_order_detail";

                var timeStamp = _auth.GetCurrentUnixTimestamp();
                var sign = _auth.GenerateSignShop(timeStamp, path, accessToken, shopId, Convert.ToInt32(_partnerId));

                const string responseOptionalFields = "buyer_user_id," +
                                                        "buyer_username,estimated_shipping_fee,recipient_address,actual_shipping_fee ," +
                                                        "goods_to_declare,note,note_update_time,item_list,pay_time," +
                                                        "dropshipper, dropshipper_phone,split_up,buyer_cancel_reason,cancel_by," +
                                                        "cancel_reason,actual_shipping_fee_confirmed,buyer_cpf_id,fulfillment_flag," +
                                                        "pickup_done_time,package_list,shipping_carrier,payment_method,total_amount," +
                                                        "buyer_username,invoice_data,no_plastic_packing,order_chargeable_weight_gram,edt";

                var url = $"{host}{path}" +
                          $"?access_token={accessToken}" +
                          $"&order_sn_list={orderSnList}" +
                          $"&partner_id={Convert.ToInt32(_partnerId)}" +
                          $"&timestamp={timeStamp}" +
                          $"&shop_id={shopId}" +
                          $"&sign={sign}" +
                          $"&response_optional_fields={responseOptionalFields}";

                var client = new RestClient(url);
                var request = new RestRequest();

                var response = await client.ExecuteGetAsync(request, cancellationToken);

                if (response is { IsSuccessful: true, Content: not null })
                {
                    var itemListResponse = JsonConvert.DeserializeObject<OrderDetailListResponse>(response.Content);

                    var errorContent = JsonConvert.DeserializeObject<AuthUserResponseError>(response.Content ?? "");

                    if (errorContent != null && (!string.IsNullOrEmpty(errorContent.Message) ||
                                                 !string.IsNullOrEmpty(errorContent.Error)))
                    {
                        Console.WriteLine($"Error: {errorContent.Message} -- {errorContent.Error}");

                        return new OrderDetailListResponse()
                        {
                            ErrorContent = errorContent
                        };
                    }

                    return itemListResponse ?? throw new Exception("Failed to deserialize response");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Integration.Shopee][External][GetOrderDetailList] API Error : {ex.Message} - {ex.StackTrace}");
                throw new Exception($"[Integration.Shopee][External][GetOrderDetailList] API Error : {ex.Message} - {ex.StackTrace}");
            }

            return new OrderDetailListResponse()
            {
                ErrorContent = new()
                {
                    Error = "Erro no método GetOrderDetailList. Response não foi obtido no request."
                }
            };
        }
    }
}
