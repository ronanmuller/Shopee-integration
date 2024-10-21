
using GE.Integration.Shopee.Domain.Response.Auth;

namespace GE.Integration.Shopee.Domain.Response.Products
{
    public class ProductListResponse
    {
        public string error { get; set; }
        public string message { get; set; }
        public string warning { get; set; }
        public string request_id { get; set; }
        public Response response { get; set; }
        public AuthUserResponseError ErrorContent { get; set; }
    }

    public class Response
    {
        public List<ResponseItem> item { get; set; }
        public int total_count { get; set; }
        public bool has_next_page { get; set; }
        public int next_offset { get; set; }
    }

    public class ResponseItem
    {
        public long item_id { get; set; }
        public string item_status { get; set; }
        public long update_time { get; set; }
    }
}
