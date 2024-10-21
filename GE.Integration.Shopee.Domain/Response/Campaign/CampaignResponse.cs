using GE.Integration.Shopee.Domain.Response.Auth;
using Newtonsoft.Json;

namespace GE.Integration.Shopee.Domain.Response.Campaign
{  
    public class CampaignResponse
    {
        [JsonProperty("request_id")]
        public string RequestId { get; set; }

        [JsonProperty("response")]
        public List<Response> Response { get; set; }

        public AuthUserResponseError? ErrorContent { get; set; }

    }

    public class Response
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("impression")]
        public int Impression { get; set; }

        [JsonProperty("clicks")]
        public int Clicks { get; set; }

        [JsonProperty("ctr")]
        public double Ctr { get; set; }

        [JsonProperty("direct_order")]
        public int DirectOrder { get; set; }

        [JsonProperty("broad_order")]
        public int BroadOrder { get; set; }

        [JsonProperty("direct_conversions")]
        public double DirectConversions { get; set; }

        [JsonProperty("broad_conversions")]
        public double BroadConversions { get; set; }

        [JsonProperty("direct_item_sold")]
        public int DirectItemSold { get; set; }

        [JsonProperty("broad_item_sold")]
        public int BroadItemSold { get; set; }

        [JsonProperty("direct_gmv")]
        public double DirectGmv { get; set; }

        [JsonProperty("broad_gmv")]
        public double BroadGmv { get; set; }

        [JsonProperty("expense")]
        public double Expense { get; set; }

        [JsonProperty("cost_per_conversion")]
        public double CostPerConversion { get; set; }

        [JsonProperty("direct_roas")]
        public double DirectRoas { get; set; }

        [JsonProperty("broad_roas")]
        public double BroadRoas { get; set; }
    }


}
