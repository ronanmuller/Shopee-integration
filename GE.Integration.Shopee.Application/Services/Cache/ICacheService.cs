using GE.Integration.Shopee.Domain.Response.WebHook;

namespace GE.Integration.Shopee.Application.Services.Cache
{
    public interface ICacheService
    {
        Task CacheToken(string token, int shopId, Guid customerId);
        Task<WebHookPost?> GetCacheToken(int shopId);
    }
}
