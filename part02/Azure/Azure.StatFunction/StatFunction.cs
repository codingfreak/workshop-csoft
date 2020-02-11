namespace commasoft.Workshop.Azure.StatFunction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    using Newtonsoft.Json;

    /// <summary>
    /// Defines the structure of the storage file in Azure BLOB storage.
    /// </summary>
    public class Stats
    {
        #region properties
        
        public List<DeviceErrorEntry> DeviceErrors { get; set; }
        
        public int MessageCount { get; set; }

        #endregion
    }

    public class DeviceErrorEntry
    {
        public string DeviceId { get; set; }

        public int ErrorsCount { get; set; }
    }

    public static class StatFunction
    {
        #region methods

        /// <summary>
        /// Retrieves the currently stored statistics from BLOB storage.
        /// </summary>
        /// <returns>The stored stats or newly generated ones.</returns>
        public static async Task<Stats> GetCurrentStatsAsync()
        {
            var blob = GetBlobReference();
            var exists = await blob.ExistsAsync();
            if (!exists)
            {
                return new Stats();
            }
            var json = await blob.DownloadTextAsync();
            var result = JsonConvert.DeserializeObject<Stats>(json);
            if (result.DeviceErrors == null)
            {
                result.DeviceErrors = new List<DeviceErrorEntry>();
            }
            return result;
        }

        /// <summary>
        /// Entry point of the Azure Function.
        /// </summary>
        /// <param name="req">The HTTP request message.</param>
        /// <param name="log">The logger to use.</param>
        /// <returns>THe HTTP respone message.</returns>
        [FunctionName("StatFunction")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("New device message arrived!");
            dynamic dataArray = await req.Content.ReadAsAsync<object>();
            if (dataArray.Count == 0)
            {
                // Return OK here so that ouput-testing in Stream Analytics works.
                log.LogInformation("No data!");
                return req.CreateResponse(HttpStatusCode.OK);
            }
            // we've got data
            try
            {
                var stats = await GetCurrentStatsAsync();
                for (var i = 0; i < dataArray.Count; i++)
                {
                    // We have to use explicit types here because of the dynamic input.
                    string deviceId = dataArray[i].DeviceId;
                    int temperature = dataArray[i].Temperature;
                    int humidity = dataArray[i].Humidity;
                    int windSpeed = dataArray[i].WindSpeed;
                    int windDirection = dataArray[i].WindDirection;
                    log.LogInformation(deviceId);
                    stats.MessageCount++;
                    var isError = temperature < -50 || temperature > 59 || humidity < 0 || humidity > 100 || windDirection < 0 || windDirection > 359 || windSpeed < 0 || windSpeed > 70;
                    if (!stats.DeviceErrors.Any(d => d.DeviceId == deviceId))
                    {
                        stats.DeviceErrors.Add(new DeviceErrorEntry { DeviceId = deviceId, ErrorsCount = isError ? 1 : 0 });
                        log.LogInformation("Added device.");
                    }
                    else
                    {
                        var errors = stats.DeviceErrors.First(d => d.DeviceId == deviceId).ErrorsCount += isError ? 1 : 0;
                        log.LogInformation("Updated device.");
                    }
                }
                await SetCurrentStatsAsync(stats);
                log.LogInformation("Stats saved.");
                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static CloudBlockBlob GetBlobReference()
        {
            // TODO 
            var account = CloudStorageAccount.Parse("CONNECTION_STRING");
            var serviceClient = account.CreateCloudBlobClient();
            var container = serviceClient.GetContainerReference("stats");
            return container.GetBlockBlobReference("stats.json");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stats"></param>
        /// <returns></returns>
        public static async Task SetCurrentStatsAsync(Stats stats)
        {
            var blob = GetBlobReference();
            var json = JsonConvert.SerializeObject(stats);
            await blob.UploadTextAsync(json);
        }

        #endregion
    }
}