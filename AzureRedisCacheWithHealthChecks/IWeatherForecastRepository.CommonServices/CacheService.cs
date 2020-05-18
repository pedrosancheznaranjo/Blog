using AzureRedisCache.CommonServices.Contracts;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace AzureRedisCache.CommonServices
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;
        private bool _redisEnabled;

        public CacheService(IDistributedCache cache, IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
            _redisEnabled = bool.Parse(_configuration.GetSection("RedisCache").GetSection("Enabled").Value);
        }

        public T GetUsingCache<T>(string cacheKey, Func<T> getFunction) where T : class
        {
            T element = null;

            if (_redisEnabled)
                element = GetElementFromCache<T>(cacheKey);

            if (element == null)
            {
                element = getFunction();

                AddElementToCache(cacheKey, element);
            }

            return element;
        }

        public async Task<T> GetUsingCache<T>(string cacheKey, Func<Task<T>> getFunction) where T : class
        {
            T element = null;

            if (_redisEnabled)
                element = GetElementFromCache<T>(cacheKey);

            if (element == null)
            {
                element = await getFunction();

                AddElementToCache(cacheKey, element);
            }

            return element;
        }

        public async Task<T> GetUsingCache<T, U>(string cacheKey, Func<U, Task<T>> getFunction, U functionParameter) where T : class
        {
            T element = null;

            if (_redisEnabled)
                element = GetElementFromCache<T>(cacheKey);

            if (element == null)
            {
                element = await getFunction(functionParameter);

                AddElementToCache(cacheKey, element);
            }

            return element;
        }

        public async Task DeleteFromCacheAsync(string cacheKey)
        {
            if (_redisEnabled)
                await _cache.RemoveAsync(cacheKey);
        }

        public void DeleteFromCache(string cacheKey)
        {
            if (_redisEnabled)
                _cache.RemoveAsync(cacheKey);
        }

        private T GetElementFromCache<T>(string cacheKey) where T : class
        {
            T element = null;

            var elementJson = _cache.GetString(cacheKey);

            if (elementJson != null)
            {
                element = JsonConvert.DeserializeObject<T>(elementJson.ToString());
            }

            return element;
        }

        private void AddElementToCache<T>(string cacheKey, T elementToStore) where T : class
        {
            if (elementToStore != null && _redisEnabled)
            {
                var elementJson = JsonConvert.SerializeObject(elementToStore);

                _cache.SetString(cacheKey, elementJson);
            }
        }

    }
}
