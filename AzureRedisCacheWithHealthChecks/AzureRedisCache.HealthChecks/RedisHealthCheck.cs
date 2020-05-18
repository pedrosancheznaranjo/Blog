using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AzureRedisCache.HealthChecks
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly IDistributedCache _cache;

        public RedisHealthCheck(IDistributedCache cache)
        {
            _cache = cache;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                _cache.GetString("health");
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception exception)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(exception.Message));
            }
        }
    }
}
