using GE.Integration.Shopee.Domain.Response.Auth;
using System.Collections.Generic;
namespace GE.Integration.Shopee.Domain.Response.Products
{

    public class ProductListDetailResponse
    {
        public string? error { get; set; }
        public string? message { get; set; }
        public string? warning { get; set; }
        public string? request_id { get; set; }
        public ResponseDetail? response { get; set; }
        public AuthUserResponseError? ErrorContent { get; set; }
    }

    public class ResponseDetail
    {
        public List<ItemList>? item_list { get; set; }
    }

    public class ItemList
    {
        public long item_id { get; set; }
        public int category_id { get; set; }
        public string item_name { get; set; } = string.Empty;
        public string? description { get; set; }
        public string? item_sku { get; set; }
        public string? gtin_code { get; set; }
        public int? create_time { get; set; } = 0;
        public int? update_time { get; set; } = 0;
        public List<AttributeList>? attribute_list { get; set; }
        public List<PriceInfo>? price_info { get; set; }
        public List<StockInfo>? stock_info { get; set; }
        public Image? image { get; set; }
        public double? weight { get; set; }
        public Dimension? dimension { get; set; }
        public List<LogisticInfo>? logistic_info { get; set; }
        public PreOrder? pre_order { get; set; }
        public List<Wholesale>? wholesales { get; set; }
        public string? condition { get; set; }
        public string? size_chart { get; set; }
        public string? item_status { get; set; }
        public string? deboost { get; set; }
        public bool? has_model { get; set; }
        public int? promotion_id { get; set; }
        public List<VideoInfo>? video_info { get; set; }
        public Brand? brand { get; set; }
        public int? item_dangerous { get; set; }
        public ComplaintPolicy? complaint_policy { get; set; }
        public TaxInfo? tax_info { get; set; }
        public DescriptionInfo? description_info { get; set; }
        public string? description_type { get; set; }
        public StockInfoV2? stock_info_v2 { get; set; }
    }

    public class AttributeList
    {
        public int? attribute_id { get; set; }
        public string? original_attribute_name { get; set; }
        public bool? is_mandatory { get; set; }
        public List<AttributeValueList>? attribute_value_list { get; set; }
    }

    public class AttributeValueList
    {
        public int? value_id { get; set; }
        public string? original_value_name { get; set; }
        public string? value_unit { get; set; }
    }

    public class Brand
    {
        public int? brand_id { get; set; }
        public string? original_brand_name { get; set; }
    }

    public class ComplaintPolicy
    {
        public string? warranty_time { get; set; }
        public bool? exclude_entrepreneur_warranty { get; set; }
        public int? complaint_address_id { get; set; }
        public string? additional_information { get; set; }
    }

    public class DescriptionInfo
    {
        public ExtendedDescription? extended_description { get; set; }
    }

    public class Dimension
    {
        public int? package_length { get; set; }
        public int? package_width { get; set; }
        public int? package_height { get; set; }
    }

    public class ExtendedDescription
    {
        public List<FieldList>? field_list { get; set; }
    }

    public class FieldList
    {
        public string? field_type { get; set; }
        public string? text { get; set; }
        public ImageInfo? image_info { get; set; }
    }

    public partial class Image
    {
        public List<string>? image_url_list { get; set; }
        public List<string>? image_id_list { get; set; }
    }

    public class ImageInfo
    {
        public string? image_id { get; set; }
        public string? image_url { get; set; }
    }

    public class LogisticInfo
    {
        public int? logistic_id { get; set; }
        public string? logistic_name { get; set; }
        public bool? enabled { get; set; }
        public double? shipping_fee { get; set; }
        public int? size_id { get; set; }
        public bool? is_free { get; set; }
        public double? estimated_shipping_fee { get; set; }
    }

    public partial class PreOrder
    {
        public bool? is_pre_order { get; set; }
        public int? days_to_ship { get; set; }

    }

    public partial class PriceInfo
    {
        public string? currency { get; set; }
        public double? original_price { get; set; }
        public double? current_price { get; set; }
        public double? inflated_price_of_original_price { get; set; }
        public double? inflated_price_of_current_price { get; set; }
        public double? sip_item_price { get; set; }
        public string? sip_item_price_source { get; set; }
    }

    public partial class SellerStock
    {
        public string? location_id { get; set; }
        public int? stock { get; set; }
        public bool if_saleable { get; set; }

    }

    public class ShopeeStock
    {
        public string? location_id { get; set; }
        public int? stock { get; set; }
    }

    public class StockInfo
    {
        public int? stock_type { get; set; }
        public string? stock_location_id { get; set; }
        public int? current_stock { get; set; }
        public int? normal_stock { get; set; }
        public int? reserved_stock { get; set; }
    }

    public partial class StockInfoV2
    {
        public SummaryInfo? summary_info { get; set; }
        public List<SellerStock>? seller_stock { get; set; }
        public List<ShopeeStock>? shopee_stock { get; set; }
    }

    public partial class SummaryInfo
    {
        public int? total_reserved_stock { get; set; }
        public int? total_available_stock { get; set; }
    }

    public class TaxInfo
    {
        public string? ncm { get; set; }
        public string? diff_state_cfop { get; set; }
        public string? csosn { get; set; }
        public string? origin { get; set; }
        public string? cest { get; set; }
        public string? measure_unit { get; set; }
        public string? invoice_option { get; set; }
        public string? vat_rate { get; set; }
        public string? hs_code { get; set; }
        public string? tax_code { get; set; }
    }

    public class VideoInfo
    {
        public string? video_url { get; set; }
        public string? thumbnail_url { get; set; }
        public int? duration { get; set; }
    }

    public class Wholesale
    {
        public int? min_count { get; set; }
        public int? max_count { get; set; }
        public double? unit_price { get; set; }
        public double? inflated_price_of_unit_price { get; set; }
    }


}

