using GE.Integration.Shopee.Domain.Response.Auth;

namespace GE.Integration.Shopee.Domain.Response.Payment
{
    using System.Collections.Generic;

    public class Item
    {
        public int activity_id { get; set; }
        public string activity_type { get; set; }
        public decimal ams_commission_fee { get; set; }
        public decimal discount_from_coin { get; set; }
        public decimal discount_from_voucher_seller { get; set; }
        public decimal discount_from_voucher_shopee { get; set; }
        public decimal discounted_price { get; set; }
        public bool is_b2c_shop_item { get; set; }
        public bool is_main_item { get; set; }
        public long item_id { get; set; }
        public string item_name { get; set; }
        public string item_sku { get; set; }
        public long model_id { get; set; }
        public string model_name { get; set; }
        public string model_sku { get; set; }
        public decimal original_price { get; set; }
        public int quantity_purchased { get; set; }
        public decimal seller_discount { get; set; }
        public decimal selling_price { get; set; }
        public decimal shopee_discount { get; set; }
    }

    public class OrderIncome
    {
        public decimal actual_shipping_fee { get; set; }
        public double buyer_paid_shipping_fee { get; set; } = 0;
        public string buyer_payment_method { get; set; }
        public decimal buyer_total_amount { get; set; }
        public decimal buyer_transaction_fee { get; set; }
        public decimal campaign_fee { get; set; }
        public decimal coins { get; set; }
        public decimal commission_fee { get; set; }
        public decimal cost_of_goods_sold { get; set; }
        public decimal credit_card_promotion { get; set; }
        public decimal credit_card_transaction_fee { get; set; }
        public decimal cross_border_tax { get; set; }
        public decimal delivery_seller_protection_fee_premium_amount { get; set; }
        public decimal drc_adjustable_refund { get; set; }
        public decimal escrow_amount { get; set; }
        public decimal escrow_tax { get; set; }
        public decimal estimated_shipping_fee { get; set; }
        public decimal final_escrow_product_gst { get; set; }
        public decimal final_escrow_shipping_gst { get; set; }
        public decimal final_product_protection { get; set; }
        public decimal final_product_vat_tax { get; set; }
        public decimal final_return_to_seller_shipping_fee { get; set; }
        public decimal final_shipping_fee { get; set; }
        public decimal final_shipping_vat_tax { get; set; }
        public decimal fsf_seller_protection_fee_claim_amount { get; set; }
        public string instalment_plan { get; set; }
        public List<Item> items { get; set; }
        public decimal order_ams_commission_fee { get; set; }
        public decimal order_chargeable_weight { get; set; }
        public decimal order_discounted_price { get; set; }
        public decimal order_original_price { get; set; }
        public decimal order_seller_discount { get; set; }
        public decimal order_selling_price { get; set; }
        public decimal original_cost_of_goods_sold { get; set; }
        public decimal original_price { get; set; }
        public decimal original_shopee_discount { get; set; }
        public decimal overseas_return_service_fee { get; set; }
        public decimal payment_promotion { get; set; }
        public decimal prorated_coins_value_offset_return_items { get; set; }
        public decimal prorated_payment_channel_promo_bank_offset_return_items { get; set; }
        public decimal prorated_payment_channel_promo_shopee_offset_return_items { get; set; }
        public decimal prorated_seller_voucher_offset_return_items { get; set; }
        public decimal prorated_shopee_voucher_offset_return_items { get; set; }
        public decimal reverse_shipping_fee { get; set; }
        public decimal reverse_shipping_fee_sst { get; set; }
        public decimal rsf_seller_protection_fee_claim_amount { get; set; }
        public decimal sales_tax_on_lvg { get; set; }
        public decimal seller_coin_cash_back { get; set; }
        public decimal seller_discount { get; set; }
        public decimal seller_lost_compensation { get; set; }
        public decimal seller_return_refund { get; set; }
        public decimal seller_shipping_discount { get; set; }
        public decimal seller_transaction_fee { get; set; }
        public List<string> seller_voucher_code { get; set; }
        public decimal service_fee { get; set; }
        public decimal shipping_fee_discount_from_3pl { get; set; }
        public decimal shipping_fee_sst { get; set; }
        public decimal shipping_seller_protection_fee_amount { get; set; }
        public double? shopee_discount { get; set; }
        public double? shopee_shipping_rebate { get; set; }
        public decimal voucher_from_seller { get; set; }
        public decimal voucher_from_shopee { get; set; }
        public decimal withholding_tax { get; set; }
    }

    public class Response
    {
        public string buyer_user_name { get; set; }
        public OrderIncome order_income { get; set; }
        public string order_sn { get; set; }
        public List<string> return_order_sn_list { get; set; }
    }

    public class EscrowDetailResponse
    {
        public string error { get; set; }
        public string message { get; set; }
        public string request_id { get; set; }
        public Response response { get; set; }
        public AuthUserResponseError? ErrorContent { get; set; }

    }
}

