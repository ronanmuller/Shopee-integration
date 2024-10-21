using GE.Integration.Shopee.Domain.Response.Auth;

namespace GE.Integration.Shopee.Domain.Response.Orders
{
    public class InvoiceData
    {
        public string number { get; set; } = string.Empty;
        public string series_number { get; set; } = string.Empty;
        public string access_key { get; set; } = string.Empty;
        public long? issue_date { get; set; }
        public float? total_value { get; set; }
        public float? products_total_value { get; set; }
        public string tax_code { get; set; } = string.Empty;

    }

    public class ImageInfo
    {
        public string image_url { get; set; } = string.Empty;
    }

    public class Item
    {
        public long item_id { get; set; } = 0;
        public string item_name { get; set; } = string.Empty;
        public string item_sku { get; set; } = string.Empty;
        public long? model_id { get; set; }
        public string model_name { get; set; } = string.Empty;
        public string model_sku { get; set; } = string.Empty;
        public long model_quantity_purchased { get; set; }
        public float? model_original_price { get; set; }
        public float? model_discounted_price { get; set; }
        public bool wholesale { get; set; }
        public float? weight { get; set; }
        public bool add_on_deal { get; set; }
        public bool main_item { get; set; }
        public int? add_on_deal_id { get; set; }
        public string promotion_type { get; set; } = string.Empty;
        public int? promotion_id { get; set; }
        public long? order_item_id { get; set; }
        public long? promotion_group_id { get; set; }
        public ImageInfo? image_info { get; set; }
        public List<string> product_location_id { get; set; }
        public bool is_prescription_item { get; set; }
        public bool is_b2c_owned_item { get; set; }
    }

    public class Parcel
    {
        public string package_number { get; set; }
        public long group_shipment_id { get; set; }
        public string logistics_status { get; set; }
        public string shipping_carrier { get; set; }
        public List<Item> item_list { get; set; }
        public long parcel_chargeable_weight_gram { get; set; }
    }

    public class RecipientAddress
    {
        public string name { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;
        public string town { get; set; } = string.Empty;
        public string district { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public string region { get; set; } = string.Empty;
        public string zipcode { get; set; } = string.Empty;
        public string full_address { get; set; } = string.Empty;
    }

    public partial class Response
    {
       // public List<Order> order_list { get; set; }
    }


    //ROOT
    public class OrderDetailListResponse
    {
        public string request_id { get; set; } = string.Empty;
        public string error { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public Response response { get; set; }
        public AuthUserResponseError? ErrorContent { get; set; }
        public string[]? warning { get; set; }

    }

    public partial class Order
    {
       // public string order_sn { get; set; } = string.Empty;
        public string region { get; set; } = string.Empty;
        public string currency { get; set; } = string.Empty;
        public bool cod { get; set; }
        public float? total_amount { get; set; }
        public string[]? pending_terms { get; set; }
        public string order_status { get; set; } = string.Empty;
        public string pay_time { get; set; } = string.Empty;
        public string shipping_carrier { get; set; } = string.Empty;
        public string payment_method { get; set; } = string.Empty;
        public float? estimated_shipping_fee { get; set; }
        public string message_to_seller { get; set; } = string.Empty;
        public string create_time { get; set; } = string.Empty;
        public long update_time { get; set; }
        public int? days_to_ship { get; set; }
        public long? ship_by_date { get; set; }
        public long? buyer_user_id { get; set; }
        public string buyer_username { get; set; } = string.Empty;
        public string buyer_cpf_id { get; set; } = string.Empty;
        public RecipientAddress recipient_address { get; set; } = new();
        public float? actual_shipping_fee { get; set; }
        public bool goods_to_declare { get; set; }
        public string note { get; set; } = string.Empty;
        public long? note_update_time { get; set; }
        public List<Item>? item_list { get; set; }
        public List<Package>? package_list { get; set; }
        public InvoiceData? invoice_data { get; set; }
        public string checkout_shipping_carrier { get; set; } = string.Empty;
        public float? reverse_shipping_fee { get; set; }
        public bool no_plastic_packing { get; set; }
        public int? order_chargeable_weight_gram { get; set; }
        public string[]? prescription_images { get; set; }
        public int? prescription_check_status { get; set; }
        public long? edt_from { get; set; }
        public long? edt_to { get; set; }
        public string fulfillment_flag { get; set; } = string.Empty;

    }

    public class Package
    {
        public string package_number { get; set; } = string.Empty;
        public string logistics_status { get; set; } = string.Empty;
        public string shipping_carrier { get; set; } = string.Empty;
    }



}
