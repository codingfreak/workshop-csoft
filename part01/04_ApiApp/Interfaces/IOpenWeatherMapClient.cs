namespace commasoft.Workshop.ApiApp.Interfaces
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IOpenWeatherMapClient
    {
        #region methods

        Task<float> GetTemperatureByCityAsync(string cityName);

        #endregion
    }
}