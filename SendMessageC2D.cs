using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using System.Text;
using Microsoft.Azure.Devices.Client;

namespace CloudFunctions
{
    public static class SendMessageC2D
    {
        [FunctionName("SendMessageC2D-UpdateStatus")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "status/{deviceId}")] HttpRequest req, string deviceId,
            ILogger log)
        {
            // enable / disable security system
            log.LogInformation("C# HTTP trigger function processed a request.");

            Console.WriteLine("Send Cloud-to-Device message\n");
            ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("iotHubEdgeConnectionString"));

            /*var commandMessage = new Message(Encoding.ASCII.GetBytes("Cloud to device message."));
            await serviceClient.SendAsync(deviceId, commandMessage);*/

            return new OkObjectResult(200);
        }

        [FunctionName("SendMessageC2D-GetStatus")]
        public static async Task<IActionResult> RunGet(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "status2/{deviceId}")] HttpRequest req, string deviceId,
            ILogger log)
        {
            // get status of security system
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                /*Console.WriteLine("Send Cloud-to-Device message\n");
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("iotHubEdgeConnectionString"));

                var commandMessage = new Message(Encoding.ASCII.GetBytes("test"));
                await serviceClient.SendAsync(deviceId, commandMessage);*/

                /*DeviceClient module_client = DeviceClient.CreateFromConnectionString("HostName=IoT-Hub-project4.azure-devices.net;DeviceId=rpi-jover;SharedAccessKey=q4V2HzFTqDGUnvoka1wLEGnLhB50b2wB381m6pRxtsk=");
                var commandMessage = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes("Cloud to device message."));
                await module_client.SendEventAsync(commandMessage);*/

                //via ServiceClient communiceren met iot hub
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString("HostName=IoT-Hub-project4.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=DSM2il5A4hLdM9ldz61ZYg26l7X+/eCAr6i+0BsPpmo=");

                //maak CloudToDeviceMethod aan en payload invullen
                CloudToDeviceMethod method = new CloudToDeviceMethod("status");
                method.SetPayloadJson("{'seconds':15}");

                //naar device sturen
                await serviceClient.InvokeDeviceMethodAsync("rpi-jover", "EdgeModule", method);
            }
            catch (Exception ex)
            {
                log.LogError("error: "+ex);
                throw;
            }
            

            return new OkObjectResult(200);
        }
    }
}
