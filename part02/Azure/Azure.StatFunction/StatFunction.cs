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

    using Newtonsoft.Json;

    public class Stats
    {
        #region properties

        public Dictionary<string, int> DeviceErrors { get; set; } = new Dictionary<string, int>();

        public int MessageCount { get; set; }

        #endregion
    }

    public static class StatFunction
    {
        #region methods

        public static async Task<Stats> GetCurrentStatsAsync()
        {
            var account = CloudStorageAccount.Parse(
                "DefaultEndpointsProtocol=https;AccountName=stoddcstest;AccountKey=61HduhogKHdWWXmQzs2Po1biEvN8HDbUehprlvdoGGawqwT+C95lpfsb7vTwQdhS3mzhCP1tZvk/CaNwhVBCQw==;EndpointSuffix=core.windows.net");
            var serviceClient = account.CreateCloudBlobClient();
            var container = serviceClient.GetContainerReference("stats");
            var blob = container.GetBlockBlobReference("stats.json");
            var exists = await blob.ExistsAsync();
            if (!exists)
            {
                return new Stats();
            }
            var json = await blob.DownloadTextAsync();
            var result = JsonConvert.DeserializeObject<Stats>(json);
            if (result.DeviceErrors == null)
            {
                result.DeviceErrors = new Dictionary<string, int>();
            }
            return result;
        }

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
                log.LogInformation("No data!");
                return req.CreateResponse(HttpStatusCode.OK);
            }
            try
            {
                var stats = await GetCurrentStatsAsync();
                for (var i = 0; i < dataArray.Count; i++)
                {
                    string deviceId = dataArray[i].DeviceId;
                    int temperature = dataArray[i].Temperature;
                    int humidity = dataArray[i].Humidity;
                    int windSpeed = dataArray[i].WindSpeed;
                    int windDirection = dataArray[i].WindDirection;
                    log.LogInformation(deviceId);
                    stats.MessageCount++;
                    var isError = temperature < -50 || temperature > 59 || humidity < 0 || humidity > 100 || windDirection < 0 || windDirection > 359 || windSpeed < 0 || windSpeed > 70;
                    if (!stats.DeviceErrors.ContainsKey(deviceId))
                    {
                        stats.DeviceErrors.Add(deviceId, isError ? 1 : 0);
                        log.LogInformation("Added device.");
                    }
                    else
                    {
                        var errors = stats.DeviceErrors[deviceId];
                        errors += isError ? 1 : 0;
                        stats.DeviceErrors[deviceId] = errors;
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

        public static async Task SetCurrentStatsAsync(Stats stats)
        {
            var account = CloudStorageAccount.Parse(
                "DefaultEndpointsProtocol=https;AccountName=stoddcstest;AccountKey=61HduhogKHdWWXmQzs2Po1biEvN8HDbUehprlvdoGGawqwT+C95lpfsb7vTwQdhS3mzhCP1tZvk/CaNwhVBCQw==;EndpointSuffix=core.windows.net");
            var serviceClient = account.CreateCloudBlobClient();
            var container = serviceClient.GetContainerReference("stats");
            var blob = container.GetBlockBlobReference("stats.json");
            var exists = await blob.ExistsAsync();
            var json = JsonConvert.SerializeObject(stats);
            await blob.UploadTextAsync(json);
        }

        #endregion
    }
}