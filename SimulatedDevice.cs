using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace simulated_device
{
    class SimulatedDevice
    {
        private static DeviceClient s_deviceClient;
        private readonly static string s_connectionString = "HostName=group7-hub.azure-devices.net;DeviceId=SimulatedDevice;SharedAccessKey=TU_CLAVE_DE_ACCESO";
        private static int s_telemetryInterval = 5; // segundos

        private static async Task<DeviceClient> ConnectIoTHubWithRetriesAsync(int maxRetries = 5, int delayMilliseconds = 2000)
        {
            int attempt = 0;
            while (true)
            {
                attempt++;
                try
                {
                    Console.WriteLine($"Intento {attempt}: Conectando al IoT Hub...");
                    // Intenta crear el DeviceClient usando el protocolo MQTT.
                    var deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString, TransportType.Mqtt);

                    // Opcionalmente, inicias algún método para probar la conexión, por ejemplo habilitando un método directo.
                    await deviceClient.SetMethodHandlerAsync("SetTelemetryInterval", SetTelemetryInterval, null);

                    Console.WriteLine("Conexión establecida correctamente");
                    return deviceClient;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Fallo en el intento {attempt}: {ex.Message}");
                    Console.ResetColor();
                    if (attempt >= maxRetries)
                    {
                        Console.WriteLine("Excedido el número máximo de reintentos. Abortando...");
                        throw; // o manejarlo según la lógica de la aplicación
                    }
                    Console.WriteLine($"Esperando {delayMilliseconds} ms antes de reintentar...");
                    await Task.Delay(delayMilliseconds);
                    // Incrementar el delay para el siguiente intento si se desea un backoff exponencial
                    delayMilliseconds *= 2;
                }
            }
        }

        // Método para manejar la llamada directa (direct method)
        private static Task<MethodResponse> SetTelemetryInterval(MethodRequest methodRequest, object userContext)
        {
            var data = Encoding.UTF8.GetString(methodRequest.Data);
            if (Int32.TryParse(data, out s_telemetryInterval))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Telemetry interval set to {0} seconds", data);
                Console.ResetColor();

                string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";
                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
            }
            else
            {
                string result = "{\"result\":\"Invalid parameter\"}";
                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
            }
        }

        // Método asíncrono para enviar telemetría simulada
        private static async Task SendDeviceToCloudMessagesAsync()
        {
            double minTemperature = 20;
            double minHumidity = 60;
            Random rand = new Random();

            while (true)
            {
                double currentTemperature = minTemperature + rand.NextDouble() * 15;
                double currentHumidity = minHumidity + rand.NextDouble() * 20;

                var telemetryDataPoint = new
                {
                    temperature = currentTemperature,
                    humidity = currentHumidity
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");

                await s_deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(s_telemetryInterval * 1000);
            }
        }

        private static async Task Main(string[] args)
        {
            Console.WriteLine("IoT Hub Quickstarts #2 - Dispositivo simulado. Ctrl-C para salir.\n");

            // Intentar conectar con reintentos
            s_deviceClient = await ConnectIoTHubWithRetriesAsync();

            // Se inicia el envío de mensajes
            var sendTask = SendDeviceToCloudMessagesAsync();

            // Se espera a que el usuario pulse Enter para salir o Ctrl+C para abortar.
            Console.ReadLine();
        }
    }
}