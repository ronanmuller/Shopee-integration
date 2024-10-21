
namespace GE.Integration.Shopee.Infra.External.Handlers
{
    public interface IAuth
    {
        string GenerateAuthUrl(string host, string path, string redirectUrl, int partnerId);
        string GenerateSignShop(long timestamp, string path, string accessToken, int shopId, int partnerId);
        public string GenerateRefreshTokenSign(int partnerId, string path, long timestamp);
        bool IsTimestampValid(long timestamp);
        long GetCurrentUnixTimestamp();
    }
}
