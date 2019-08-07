namespace commasoft.Workshop.ApiApp.Clients
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Interfaces;

    using Microsoft.Extensions.Configuration;

    using Models;

    using Newtonsoft.Json;

    public class OpenWeatherMapClient : IOpenWeatherMapClient
    {
        #region constructors and destructors

        public OpenWeatherMapClient(HttpClient client, IConfiguration configuration)
        {
            Client = client;
            Configuration = configuration;
            ConfigureClient();
        }

        #endregion

        #region explicit interfaces

        public async Task<float> GetTemperatureByCityAsync(string cityName)
        {
            var address = $"weather?q={cityName}&units=metric&lang=de&appId={Configuration["External:OpenWeatherMap:ApiKey"]}";
            try
            {
                var response = await Client.GetAsync(address);
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException("Could not retrieve result from OpenWeatherMap API.");
                }
                var result = await response.Content.ReadAsStringAsync();
                var converted = JsonConvert.DeserializeObject<OpenWeatherResultModel>(result);
                if (converted == null)
                {
                    throw new InvalidOperationException("Unexpected data retrieved from OpenWeatherMap API.");
                }
                return converted.main.temp;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unknown error during API request.", ex);
            }
        }

        #endregion

        #region methods

        private void ConfigureClient()
        {
            Client.BaseAddress = new Uri($"{Configuration["External:OpenWeatherMap:BaseUrl"]}/{Configuration["External:OpenWeatherMap:ApiVersion"]}/");
        }

        #endregion

        #region properties

        private HttpClient Client { get; }

        private IConfiguration Configuration { get; }

        #endregion
    }
}