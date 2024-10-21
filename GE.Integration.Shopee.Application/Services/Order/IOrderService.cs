using GE.Integration.Shopee.Domain.Response.Orders;
using GE.Integration.Shopee.Domain.Response.Shop;
using GE.Integration.Shopee.Domain.Response.WebHook;
using Microsoft.AspNetCore.Mvc;

namespace GE.Integration.Shopee.Application.Services.Order
{
    public interface IOrderService
    {
        Task<ShopInfoResponse> GetShopDetail(string accessToken, int shopId);

        Task<IActionResult> GetOrderList(Guid customerId, long? customerPlanId, string accessToken, int shopId, DateTime dateCreatedFrom, DateTime dateCreatedTo, CancellationToken cancellationToken);

        Task<string> GetOrderDetail(WebHookPost webHookContentMessage, CancellationToken cancellationToken);

        Task PostWebhookShopee(OrderWebHookShopeePost request, CancellationToken cancellationToken);

        Task<bool> DeleteLocalBiggerThanXDays(int days);
    }
}
