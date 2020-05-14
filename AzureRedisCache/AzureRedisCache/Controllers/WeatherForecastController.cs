using AzureRedisCache.CommonServices.Contracts;
using AzureRedisCache.Repository.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace AzureRedisCache.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherForecastRepository _weatherForecastRepository;
        private readonly ICacheService _cacheService;

        public WeatherForecastController(IWeatherForecastRepository weatherForecastRepository, 
                                         ICacheService cacheService,
                                         ILogger<WeatherForecastController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _weatherForecastRepository = weatherForecastRepository ?? throw new ArgumentNullException(nameof(weatherForecastRepository));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        [HttpGet("Get")]
        public IEnumerable<WeatherForecast> Get()
        {
            return _weatherForecastRepository.GetForecast();
        }

        [HttpGet("GetCached")]
        public IEnumerable<WeatherForecast> GetCached()
        {
            return _cacheService.GetUsingCache("TestCache", () => _weatherForecastRepository.GetForecast());
        }
    }
}
