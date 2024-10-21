using GE.Integration.Shopee.Domain.Response.Products;

namespace GE.Integration.Shopee.Infra.External.Products
{
    public interface IGetProductsExternal
    {
        Task<ProductListDetailResponse> GetItemDetail(string accessToken, int shopId, long itemId, CancellationToken cancellationToken);
        Task<ProductListResponse> GetItemList(string accessToken, int shopId, DateTime dateCreatedFrom, DateTime dateCreatedTo, CancellationToken cancellationToken, int page, int pageSize);
        Task<ProductCategoryResponse> GetCategory(string accessToken, int shopId, CancellationToken cancellationToken);
        Task<ProductExtraInfoResponse> GetItemExtraInfo(string accessToken, int shopId, CancellationToken cancellationToken, long itemId);
        Task<ProductModelListResponse> GetModelItemList(string accessToken, int shopId, long itemId, CancellationToken cancellationToken);
    }
}

