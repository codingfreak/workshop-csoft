namespace commasoft.Workshop.ApiApp.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Interfaces;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        #region constructors and destructors

        public WeatherController(IOpenWeatherMapClient openWeatherMapClient)
        {
            OpenWeatherMapClient = openWeatherMapClient;
        }

        #endregion

        #region methods

        [HttpGet]
        [Route("{cityName}/temperature")]
        public async Task<ActionResult<IEnumerable<float>>> GetTemperatureByCity(string cityName)
        {
            var result = await OpenWeatherMapClient.GetTemperatureByCityAsync(cityName);
            return Ok(result);
        }

        #endregion

        #region properties

        private IOpenWeatherMapClient OpenWeatherMapClient { get; }

        #endregion
    }
}