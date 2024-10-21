using GE.Integration.Shopee.Domain.Enum;

namespace GE.Integration.Shopee.Domain.Response.WebHook
{
    public class WebHookPost
    {
        public Guid customerId { get; set; }
        public long? customerPlanId { get; set; }
        public string accessToken { get; set; }
        public int shopId { get; set; }
        public string itemIds { get; set; }
        public string hashIntegration { get; set; }
        public ETypeEntity EntityType { get; set; }
        public Dictionary<string, string> status { get; set; }

    }
}
