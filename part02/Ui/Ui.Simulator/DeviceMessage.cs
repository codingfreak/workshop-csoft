namespace commasoft.Workshop.Ui.Simulator
{
    using System;
    using System.Linq;

    /// <summary>
    /// Defines the model for a single Device-2-Cloud-message.
    /// </summary>
    public class DeviceMessage
    {
        #region properties

        public string Id { get; } = Guid.NewGuid().ToString();

        public string DeviceId { get; set; }

        /// <summary>
        /// The humidity in %.
        /// </summary>
        public int Humidity { get; set; }

        /// <summary>
        /// The temperatore in °C.
        /// </summary>
        public int Temperature { get; set; }

        /// <summary>
        /// This wind direction in °.
        /// </summary>
        public int WindDirection { get; set; }

        /// <summary>
        /// The windspeed in m/s.
        /// </summary>
        public int WindSpeed { get; set; }

        #endregion
    }
}