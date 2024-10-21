using GE.Integration.Shopee.Domain.Response.Orders;

namespace GE.Integration.Shopee.Infra.External.Orders
{
    public interface IGetOrdersExternal
    {
        Task<OrderListResponse> GetOrderList(string accessToken, int shopId, DateTime dateCreatedFrom, DateTime dateCreatedTo, CancellationToken cancellationToken, string cursor, int pageSize);

        Task<OrderDetailListResponse> GetOrderDetailList(string accessToken, int shopId, string order_sn_list, CancellationToken cancellationToken);
    }
}
