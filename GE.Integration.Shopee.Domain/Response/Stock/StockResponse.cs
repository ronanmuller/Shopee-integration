using GE.Integration.Shopee.Domain.Response.Products;

namespace GE.Integration.Shopee.Domain.Response.Stock
{
    public class StockResponse
    {
        public List<StockInfo>? stock_info { get; set; }
        public StockInfoV2? stock_info_v2 { get; set; }

    }
}
