using GE.Integration.Shopee.Domain.Response.WebHook;
using Microsoft.AspNetCore.Mvc;

namespace GE.Integration.Shopee.Application.Services.Product
{
    public interface IProductService
    {
        Task<IActionResult> GetItemList(Guid customerId, string accessToken, int shopId, DateTime dateCreatedFrom,
            DateTime dateCreatedTo,
            long? customerPlanId, CancellationToken cancellationToken);
        Task<Contracts.DomainModels.Products.Product> GetItemDetail(WebHookPost webhookMessage);

    }
}
