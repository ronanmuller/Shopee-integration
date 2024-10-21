namespace GE.Integration.Shopee.Domain.Response.Orders
{
    public class OrderWebHookShopeePost
    {
        public Data data { get; set; }
        public int shop_id { get; set; }
        public int code { get; set; }
        public long timestamp { get; set; }
       
    }

    public class Data
    {
        public List<object>? items { get; set; }
        public string ordersn { get; set; }
        public string status { get; set; }
        public string completed_scenario { get; set; }
        public long update_time { get; set; }
    }
}
