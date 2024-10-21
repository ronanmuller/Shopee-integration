using Amazon;
using Amazon.ElastiCache;
using Amazon.ElastiCache.Model;
using StackExchange.Redis;


namespace GE.Integration.Shopee.Application.Helpers
{
    public class CacheManagerHelper
    {
        private readonly IAmazonElastiCache _elasticacheClient;
        private ConnectionMultiplexer _redisConnection;

        public CacheManagerHelper()
        {
            Console.WriteLine("Inicializando o CacheManagerHelper");

            var regionEndpoint = RegionEndpoint.USEast1;
            var config = new AmazonElastiCacheConfig
            {
                RegionEndpoint = regionEndpoint
            };
            _elasticacheClient = new AmazonElastiCacheClient(config);
        }

        public async Task InitializeRedisConnectionAsync(string clusterEndpoint, string endpointUrl)
        {
            try
            {
                Console.WriteLine("Inicializando o cluster: " + clusterEndpoint);

                var response = await _elasticacheClient.DescribeCacheClustersAsync(new DescribeCacheClustersRequest
                {
                    CacheClusterId = clusterEndpoint
                });

                if (response.CacheClusters.Count > 0)
                {
                    var options = new ConfigurationOptions
                    {
                        EndPoints = { $"{endpointUrl}" }
                    };
                    _redisConnection = await ConnectionMultiplexer.ConnectAsync(options);
                }
                else
                {
                    Console.WriteLine("Cluster ElastiCache não encontrado ou sem clusters disponíveis.");
                    throw new Exception("Cluster ElastiCache não encontrado ou sem clusters disponíveis.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar a conexão com o Redis: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        public async Task<string> GetValueAsync(string key)
        {
            try
            {
                if (_redisConnection == null)
                {
                    throw new InvalidOperationException("Conexão com o Redis não inicializada.");
                }

                var db = _redisConnection.GetDatabase();
                return await db.StringGetAsync(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter valor do cache: {ex.Message}");
                return null;
            }
        }

        public async Task SetValueAsync(string key, string value)
        {
            try
            {
                if (_redisConnection == null)
                {
                    Console.WriteLine("Conexão com o Redis não inicializada.");
                    throw new InvalidOperationException("Conexão com o Redis não inicializada.");
                }

                var db = _redisConnection.GetDatabase();
                await db.StringSetAsync(key, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao definir valor no cache: {ex.Message}");
            }
        }

        public async Task RemoveValueAsync(string key)
        {
            try
            {
                if (_redisConnection == null)
                {
                    throw new InvalidOperationException("Conexão com o Redis não inicializada.");
                }

                var db = _redisConnection.GetDatabase();
                await db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao remover valor do cache: {ex.Message}");
                throw;
            }
        }
    }
}
