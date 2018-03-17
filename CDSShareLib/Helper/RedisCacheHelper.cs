using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CDSShareLib.Helper
{
    public class RedisCacheHelper
    {
        private Lazy<ConnectionMultiplexer> LazyConnection;
        private ConnectionMultiplexer _Connection;
        private IDatabase _RedisCache;
        private static string connectionString = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("RedisCacheConnectionString");
        public RedisCacheHelper()
        {            
            LazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString));
            _Connection = LazyConnection.Value;
            _RedisCache = _Connection.GetDatabase();
        }
        
        public string GetValueByKey(RedisKey cacheKey)
        {
            string keyValue = _RedisCache.StringGet(cacheKey);
            if (string.IsNullOrEmpty(keyValue))
                return null;
            else
                return keyValue;
        }
        public void SetKeyValue(RedisKey cacheKey, string cacheValue)
        {
            _RedisCache.StringSet(cacheKey, cacheValue);
        }

        public void SetKeyValue(RedisKey cacheKey, string cacheValue, TimeSpan expiredTimeSpan)
        {
            _RedisCache.StringSet(cacheKey, cacheValue, expiredTimeSpan);
        }

        public NameValueCollection SearchCacheKey(string keyword)
        {
            NameValueCollection cacheObject = new NameValueCollection();

            EndPoint[] redisServerEndPoints = _Connection.GetEndPoints();
            foreach (var serverEndPoint in redisServerEndPoints)
            {
                var server = _Connection.GetServer(serverEndPoint);
                foreach (var key in server.Keys(pattern: keyword))
                {
                    cacheObject.Add(key, GetValueByKey(key));
                }
            }

            return cacheObject;
        }

        public void PurgeKey(RedisKey cacheKey)
        {
            _RedisCache.KeyDelete(cacheKey);
        }
    }
}
