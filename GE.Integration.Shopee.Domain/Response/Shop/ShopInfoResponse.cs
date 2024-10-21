using GE.Integration.Shopee.Domain.Response.Auth;

namespace GE.Integration.Shopee.Domain.Response.Shop
{
    public class ShopInfoResponse
    {
        public string shop_name { get; set; }
        public string region { get; set; }
        public string status { get; set; }
        public bool is_sip { get; set; }
        public bool is_cb { get; set; }
        public bool is_cnsc { get; set; }
        public string request_id { get; set; }
        public int auth_time { get; set; }
        public int expire_time { get; set; }
        public string error { get; set; }
        public string message { get; set; }
        public string shop_cbsc { get; set; }
        public string mtsku_upgraded_status { get; set; }
        public AuthUserResponseError? ErrorContent { get; set; }

    }
}
