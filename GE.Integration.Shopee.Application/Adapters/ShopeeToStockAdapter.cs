using GE.Contracts.DomainModels.Interfaces;
using Prod = GE.Integration.Shopee.Domain.Response.Products;

namespace GE.Integration.Shopee.Application.Adapters
{
    public class ShopeeToStockAdapter : IStockAdapter
    {
        private readonly Prod.Model _response;
        private readonly string _idItem;

        public ShopeeToStockAdapter(Prod.Model response, string idItem)
        {
            _response = response;
            _idItem = idItem;
        }

        public long? InternalId() => Convert.ToInt64(_idItem);

        public string ProductExternalId() => _idItem;

        public string? Name()
        {
            return _response.stock_info_v2.seller_stock is { Count: > 0 } ? _response.stock_info_v2.seller_stock[0].location_id : "Geral";
        }

        public int? Quantity()
        {
            if (_response != null &&
                _response.stock_info_v2 != null &&
                _response.stock_info_v2.seller_stock != null &&
                _response.stock_info_v2.seller_stock.Count > 0)
            {
                return _response.stock_info_v2.seller_stock[0].stock;
            }

            if (_response != null &&
                _response.stock_info_v2 != null &&
                _response.stock_info_v2.summary_info != null)
            {
                return _response.stock_info_v2.summary_info?.total_available_stock;
            }

            return 0;
        }

        public bool IsActive() => true;

        public long VariationId() => 0;

        public string? Company() => string.Empty;
    }
}
