namespace commasoft.Workshop.ApiApp.Models
{
    using System;
    using System.Linq;

    public class OpenWeatherResultModel
    {
        #region properties

        public string _base { get; set; }

        public int cod { get; set; }

        public int dt { get; set; }

        public int id { get; set; }

        public string name { get; set; }

        public OpenWeatherCloudsData clouds { get; set; }

        public OpenWeatherCoordinates coord { get; set; }

        public OpenWeatherMainData main { get; set; }

        public OpenWeatherSystemData sys { get; set; }

        public OpenWeatherWeatherData[] weather { get; set; }

        public OpenWeatherWindData wind { get; set; }

        public int timezone { get; set; }

        public int visibility { get; set; }

        #endregion
    }

    public class OpenWeatherCoordinates
    {
        #region properties

        public float lat { get; set; }

        public float lon { get; set; }

        #endregion
    }

    public class OpenWeatherMainData
    {
        #region properties

        public float humidity { get; set; }

        public float pressure { get; set; }

        public float temp { get; set; }

        public float temp_max { get; set; }

        public float temp_min { get; set; }

        #endregion
    }

    public class OpenWeatherWindData
    {
        #region properties

        public int deg { get; set; }

        public float speed { get; set; }

        #endregion
    }

    public class OpenWeatherCloudsData
    {
        #region properties

        public int all { get; set; }

        #endregion
    }

    public class OpenWeatherSystemData
    {
        #region properties

        public string country { get; set; }

        public int id { get; set; }

        public float message { get; set; }

        public int sunrise { get; set; }

        public int sunset { get; set; }

        public int type { get; set; }

        #endregion
    }

    public class OpenWeatherWeatherData
    {
        #region properties

        public string description { get; set; }

        public string icon { get; set; }

        public int id { get; set; }

        public string main { get; set; }

        #endregion
    }
}