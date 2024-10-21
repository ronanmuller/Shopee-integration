using System.Security.Cryptography;
using System.Text;


namespace GE.Integration.Shopee.Infra.External.Handlers
{
    public class Auth : IAuth
    {
        private static readonly string PartnerKey = Environment.GetEnvironmentVariable("PARTNER_KEY") ?? "53766e58554a445a52764d4f444c645475584c63426e626b764f716c4d74476e";

        public string GenerateAuthUrl(string host, string path, string redirectUrl, int partnerId)
        {
            var timestamp = GetCurrentUnixTimestamp();
            var baseString = $"{partnerId}{path}{timestamp}";
            var partnerKeyBytes = Encoding.UTF8.GetBytes(PartnerKey);
            var sign = CalculateHmac(baseString, partnerKeyBytes);

            if (!string.IsNullOrEmpty(redirectUrl))
                return $"{host}{path}?partner_id={partnerId}&timestamp={timestamp}&sign={sign}&redirect={redirectUrl}";

            return $"{host}{path}?partner_id={partnerId}&timestamp={timestamp}&sign={sign}";
        }

        public string GenerateSignShop(long timestamp, string path, string accessToken, int shopId, int partnerId)
        {
            var baseString = $"{partnerId}{path}{timestamp}{accessToken}{shopId}";
            var baseStringBytes = Encoding.UTF8.GetBytes(baseString);
            var partnerKeyBytes = Encoding.UTF8.GetBytes(PartnerKey);
           
            using HMACSHA256 hmac = new HMACSHA256(partnerKeyBytes);
         
            var hashBytes = hmac.ComputeHash(baseStringBytes);

            return string.Concat(Array.ConvertAll(hashBytes, b => b.ToString("x2")));
        }

        public string GenerateRefreshTokenSign(int partnerId, string path, long timestamp)
        {
            var baseString = $"{partnerId}{path}{timestamp}{PartnerKey}";
            var baseStringBytes = Encoding.UTF8.GetBytes(baseString);
            var partnerKeyBytes = Encoding.UTF8.GetBytes(PartnerKey);

            using HMACSHA256 hmac = new HMACSHA256(partnerKeyBytes);

            var hashBytes = hmac.ComputeHash(baseStringBytes);

            return string.Concat(Array.ConvertAll(hashBytes, b => b.ToString("x2")));
        }

        public bool IsTimestampValid(long timestamp)
        {
            const int maxDifferenceSeconds = 5 * 60;

            var currentTimestamp = GetCurrentUnixTimestamp();
            var differenceSeconds = currentTimestamp - timestamp;

            return differenceSeconds >= 0 && differenceSeconds <= maxDifferenceSeconds;
        }

        public long GetCurrentUnixTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        private string CalculateHmac(string input, byte[] keyBytes)
        {
            using HMACSHA256 hmac = new HMACSHA256(keyBytes);

            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
