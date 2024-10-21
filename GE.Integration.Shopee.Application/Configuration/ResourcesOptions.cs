namespace GE.Integration.Shopee.Application.Configuration
{
    public class ResourcesOptions
    {
        public const string Resources = "Resources";

        public string ApiShopeeRequestToken { get; set; } = String.Empty;
        public string ApiShopee { get; set; } = String.Empty;
        public string AuthCallback { get; set; } = String.Empty;
        public string AuthUrlHost { get; set; } = String.Empty;
    }
}
