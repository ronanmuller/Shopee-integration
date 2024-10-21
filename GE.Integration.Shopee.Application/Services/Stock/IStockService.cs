using GE.Integration.Shopee.Domain.Response.WebHook;
using Microsoft.AspNetCore.Mvc;

namespace GE.Integration.Shopee.Application.Services.Stock
{
    public interface IStockService
    {
        Task<IActionResult> GetItemList(string accessToken, DateTime dateCreatedFrom, DateTime dateCreatedTo, int shopId, CancellationToken cancellationToken);

        Task<string> GetItemDetail(WebHookPost webhookMessage, CancellationToken cancellationToken);
    }
}
