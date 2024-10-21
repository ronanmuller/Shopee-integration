using GE.Integration.Shopee.Application.Helpers;
using GE.Integration.Shopee.Domain.Response.WebHook;
using Newtonsoft.Json;

namespace GE.Integration.Shopee.Application.Services.Cache
{
    public class CacheService : ICacheService
    {
        private readonly CacheManagerHelper _cacheManager;
        private readonly string AWS_REDIS_CLUSTERNAME = Environment.GetEnvironmentVariable("AWS_REDIS_CACHE_CUSTOMERAPPLICATIONS_CLUSTERNAME");
        private readonly string AWS_REDIS_ENPOINT_URL = Environment.GetEnvironmentVariable("AWS_REDIS_CACHE_CUSTOMERAPPLICATIONS");
        public CacheService(CacheManagerHelper cacheManager)
        {
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// FAz o cache do token para ser usado pelos weebhooks da shopee
        /// </summary>
        /// <param name="token"></param>
        /// <param name="shopId"></param>
        /// <param name="customerId"></param>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public async Task CacheToken(string token, int shopId, Guid customerId)
        {
            try
            {
                // Inicializa
                await _cacheManager.InitializeRedisConnectionAsync(AWS_REDIS_CLUSTERNAME, AWS_REDIS_ENPOINT_URL);
                var orderWebHookPost = new WebHookPost
                {
                    accessToken = token,
                    shopId = shopId,
                    customerId = customerId
                };

                var shoppeAppIntegration = JsonConvert.SerializeObject(orderWebHookPost);
                await _cacheManager.SetValueAsync(shopId.ToString(), shoppeAppIntegration);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao salvar os dados no cache. - {e.Message}");
            }
        }

        /// <summary>
        /// Obtem o token com base no id do shop
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="customerId"></param>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public async Task<WebHookPost?> GetCacheToken(int shopId)
        {
            WebHookPost? shoppeAppIntegration = null;

            // Inicializa
            await _cacheManager.InitializeRedisConnectionAsync(AWS_REDIS_CLUSTERNAME, AWS_REDIS_ENPOINT_URL);

            // Atribui o valor
            var customerAppIntegrationStr = await _cacheManager.GetValueAsync(shopId.ToString());

            Console.WriteLine(customerAppIntegrationStr);

            if (!string.IsNullOrEmpty(customerAppIntegrationStr))
            {
                // converte o cache no objeto lista
                shoppeAppIntegration = JsonConvert.DeserializeObject<WebHookPost>(customerAppIntegrationStr);
            }

            return shoppeAppIntegration;
        }
    }
}
