using AzureRedisCache.CommonServices;
using AzureRedisCache.CommonServices.Contracts;
using AzureRedisCache.HealthChecks;
using AzureRedisCache.Repository;
using AzureRedisCache.Repository.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

namespace AzureRedisCache.API.Extensions
{
    public static class IServiceCollectionsExtensions
    {
        public static IServiceCollection AddCustomFilters(this IServiceCollection services)
        {
            return services
                    .AddControllers()
                    .Services;
        }

        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddCheck<RedisHealthCheck>("Redis")
                .Services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                     .AddTransient<IWeatherForecastRepository, WeatherForecastRepository>();
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services) =>
         services
         .AddSwaggerGen(c =>
         {
             c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
         });

        public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                    .AddTransient<ICacheService, CacheService>()
                    .AddStackExchangeRedisCache(option =>
                     {
                         option.Configuration = configuration.GetConnectionString("RedisConnectionString");
                     });
        }
    }
}
