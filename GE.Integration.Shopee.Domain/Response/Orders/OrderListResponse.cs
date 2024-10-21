using GE.Integration.Shopee.Domain.Response.Auth;

namespace GE.Integration.Shopee.Domain.Response.Orders
{
    public class OrderListResponse
    {
        public string error { get; set; }
        public string message { get; set; }
        public Response response { get; set; }
        public string request_id { get; set; }
        public AuthUserResponseError ErrorContent { get; set; }

    }

    public partial class Response
    {
        public bool more { get; set; }
        public string next_cursor { get; set; }
        public List<Order> order_list { get; set; }
    }

    public partial class Order
    {
        public string order_sn { get; set; }
    }
}
