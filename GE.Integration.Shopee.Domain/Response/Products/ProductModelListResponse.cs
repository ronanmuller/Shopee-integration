
using GE.Integration.Shopee.Domain.Response.Auth;

namespace GE.Integration.Shopee.Domain.Response.Products
{
    public partial class SummaryInfo
    {
        //public int total_reserved_stock { get; set; }
        //public int total_available_stock { get; set; }
    }

    public class OptionList
    {
        public string option { get; set; }
    }

    public class TierVariation
    {
        public string name { get; set; }
        public List<OptionList> option_list { get; set; }
    }

    public partial class PriceInfo
    {
        //public int current_price { get; set; }
        //public int original_price { get; set; }
        //public int inflated_price_of_current_price { get; set; }
        //public int inflated_price_of_original_price { get; set; }
    }

    public partial class PreOrder
    {
        //public bool is_pre_order { get; set; }
        //public int days_to_ship { get; set; }
    }

    public partial class SellerStock
    {
        //public string location_id { get; set; }
        //public int stock { get; set; }
    }

    public partial class StockInfoV2
    {
        //public SummaryInfo summary_info { get; set; }
        //public List<SellerStock> seller_stock { get; set; }
    }

    public class Model
    {
        public long model_id { get; set; }
        public string model_status { get; set; }
        public int promotion_id { get; set; }
        public List<int> tier_index { get; set; }
        public List<PriceInfo> price_info { get; set; }
        public string model_sku { get; set; }
        public string gtin_code { get; set; }
        public PreOrder pre_order { get; set; }
        public StockInfoV2 stock_info_v2 { get; set; }
    }

    public class ResponseModelDetail
    {
        public List<TierVariation> tier_variation { get; set; }
        public List<Model> model { get; set; }
    }

    public class ProductModelListResponse
    {
        public ResponseModelDetail Response { get; set; }
        public AuthUserResponseError? ErrorContent { get; set; }
        public string? error { get; set; }
        public string? message { get; set; }
        public string? warning { get; set; }
        public string? request_id { get; set; }
    }


}


