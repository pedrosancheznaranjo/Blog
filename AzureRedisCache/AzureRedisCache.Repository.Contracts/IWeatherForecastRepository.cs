using System;
using System.Collections.Generic;

namespace AzureRedisCache.Repository.Contracts
{
    public interface IWeatherForecastRepository
    {
        IEnumerable<WeatherForecast> GetForecast();
    }
}
