namespace commasoft.Workshop.Ui.Simulator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Azure.Devices.Client;

    using Newtonsoft.Json;

    internal class Program
    {
        #region constants

        /// <summary>
        /// Client for sending the message to the Iot hub.
        /// </summary>
        private static DeviceClient _deviceClient;

        /// <summary>
        /// The key of the device.
        /// </summary>
        private static string _deviceKey;

        private static int _errorRate;

        /// <summary>
        /// Simulated event frequency.
        /// </summary>
        private static int _eventFrequency = 1000;

        /// <summary>
        /// Endpoint to the iot hub.
        /// </summary>
        private static string _iotHubEndpoint;

        /// <summary>
        /// The key of the machine.
        /// </summary>
        private static string _machineKey;

        private static int _messageCount;

        /// <summary>
        /// TokenSource for the cancellation token.
        /// </summary>
        private static readonly CancellationTokenSource TokenSource = new CancellationTokenSource();

        #endregion

        #region methods

        private static async Task Main(string[] args)
        {
            Console.WriteLine("Reading environment...");
            _machineKey = Environment.GetEnvironmentVariable("SENSORSIM_MACHINEKEY");
            _deviceKey = Environment.GetEnvironmentVariable("SENSORSIM_DEVICEKEY");
            _iotHubEndpoint = Environment.GetEnvironmentVariable("SENSORSIM_ENDPOINT");
            var eventFrequencyValue = Environment.GetEnvironmentVariable("SENSORSIM_WAITTIME");
            var errorRate = Environment.GetEnvironmentVariable("SENSORSIM_ERRORRATE");
            if (!int.TryParse(eventFrequencyValue, out _eventFrequency))
            {
                throw new ArgumentException("Event frequency is missing or invalid.");
            }
            if (!int.TryParse(errorRate, out _errorRate))
            {
                throw new ArgumentException("Error rate is missing or invalid.");
            }
            Console.WriteLine("Starting simulator.");
            try
            {
                Task.Run(
                    () =>
                    {
                        var key = Console.ReadKey();
                        if (key.Key == ConsoleKey.Q)
                        {
                            TokenSource.Cancel();
                        }
                    }).ContinueWith(
                    t =>
                    {
                        Console.WriteLine("Program exited.");
                    });
                await RunSimulatorAsync(TokenSource.Token).ConfigureAwait(false);
            }
            finally
            {
                _deviceClient?.Dispose();
                Console.WriteLine("Simulator finished.");
            }
            Console.WriteLine("Program finished.");
        }

        /// <summary>
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static async Task RunSimulatorAsync(CancellationToken token = default)
        {
            if (!TryCreateDeviceClient())
            {
                // Device client could not be created.
                return;
            }
            var random = new Random(DateTime.Now.Millisecond);
            // Start incoming simulated events.
            while (!token.IsCancellationRequested)
            {
                var isErrorMessage = false;
                try
                {
                    var nextMessage = new DeviceMessage
                    {
                        DeviceId = _machineKey,
                        Temperature = random.Next(-50, 50),
                        WindSpeed = random.Next(0, 70),
                        WindDirection = random.Next(0, 359),
                        Humidity = random.Next(0, 100)
                    };
                    _messageCount++;
                    if (_messageCount % _errorRate == 0)
                    {
                        // TODO more sophisticated!
                        nextMessage.Temperature = 80;
                        isErrorMessage = true;
                    }
                    var converted = JsonConvert.SerializeObject(nextMessage);
                    var messageBytes = Encoding.UTF8.GetBytes(converted);
                    await _deviceClient.SendEventAsync(new Message(messageBytes), token).ConfigureAwait(false);
                    Console.WriteLine($"Message #{_messageCount} sent {(isErrorMessage ? "FEHLER" : "")}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: #{ex.Message}.");
                    if (ex is SocketException)
                    {
                        // Device client has trouble sending.
                        if (!TryCreateDeviceClient())
                        {
                            // Device client could not be created.
                            TokenSource.Cancel();
                        }
                    }
                    else
                    {
                        // Unknown sending error.
                        TokenSource.Cancel();
                    }
                    Console.WriteLine("Error: Could not send data: {0}", ex.Message);
                    Console.WriteLine(ex);
                }
                await Task.Delay(TimeSpan.FromSeconds(_eventFrequency), token);
            }
        }

        /// <summary>
        /// Tries creating a device client and connecting it to the IoT hub.
        /// </summary>
        /// <returns><c>True</c> if the connection was established.</returns>
        private static bool TryCreateDeviceClient()
        {
            Console.WriteLine("Connecting to Iot hub...");
            var result = false;
            try
            {
                _deviceClient = DeviceClient.Create(_iotHubEndpoint, new DeviceAuthenticationWithRegistrySymmetricKey(_machineKey, _deviceKey), TransportType.Http1);
                _deviceClient.SetRetryPolicy(new NoRetry());
                _deviceClient.OperationTimeoutInMilliseconds = 1000;
                result = true;
                Console.WriteLine("Iot hub connected.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: Could not connect to IoT hub: {0}", ex.Message);
            }
            return result;
        }

        #endregion
    }
}