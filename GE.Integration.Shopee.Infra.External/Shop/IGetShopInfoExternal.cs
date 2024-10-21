using GE.Integration.Shopee.Domain.Response.Shop;

namespace GE.Integration.Shopee.Infra.External.Shop
{
    public interface IGetShopInfoExternal
    {
        Task<ShopInfoResponse> GetShopInfoDetails(string accessToken, int shopId);
    }
}
