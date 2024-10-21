using GE.Integration.Shopee.Domain.Response.Auth;

namespace GE.Integration.Shopee.Domain.Response.Products
{
    public class ExtraInfoItemList
    {
        public long item_id { get; set; }
        public int sale { get; set; }
        public int views { get; set; }
        public int likes { get; set; }
        public int rating_star { get; set; }
        public int comment_count { get; set; }

    }

    public class ExtraInfoResponse
    {
        public List<ExtraInfoItemList> item_list { get; set; }
    }

    public class ProductExtraInfoResponse
    {
        public string error { get; set; }
        public string message { get; set; }
        public string warning { get; set; }
        public string request_id { get; set; }
        public ExtraInfoResponse response { get; set; }
        public AuthUserResponseError? ErrorContent { get; set; }

    }
}
