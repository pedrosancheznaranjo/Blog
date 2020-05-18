using System;
using System.Threading.Tasks;

namespace AzureRedisCache.CommonServices.Contracts
{
    public interface ICacheService
    {
        T GetUsingCache<T>(string cacheKey, Func<T> getFunction) where T : class;

        Task<T> GetUsingCache<T>(string cacheKey, Func<Task<T>> getFunction) where T : class;

        Task<T> GetUsingCache<T, U>(string cacheKey, Func<U, Task<T>> getFunction, U functionParameter) where T : class;

        Task DeleteFromCacheAsync(string cacheKey);

        void DeleteFromCache(string cacheKey);
    }
}
