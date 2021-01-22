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
using System.Collections.Generic;

namespace CloudFunctions
{
    public static class SendDirectMethod
    {
        [FunctionName("SendDirectMethod-UpdateStatus")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "status/{deviceId}")] HttpRequest req, string deviceId,
            ILogger log)
        {
            // enable / disable security system
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                //body van post lezen
                string body = await new StreamReader(req.Body).ReadToEndAsync();
                //var json = Newtonsoft.Json.JsonConvert.DeserializeObject(body);

                //via ServiceClient communiceren met iot hub
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("iotHubEdgeDeviceConnectionString"));

                //maak CloudToDeviceMethod aan en payload invullen
                CloudToDeviceMethod method = new CloudToDeviceMethod("updateStatus");
                method.SetPayloadJson(body);

                //naar device sturen
                var response = await serviceClient.InvokeDeviceMethodAsync(deviceId, "EdgeModule", method);
            }
            catch (Exception ex)
            {
                log.LogError("error: " + ex);
                throw;
            }

            return new OkObjectResult(200);
        }

        [FunctionName("SendDirectMethod-GetStatus")]
        public static async Task<Dictionary<string, string>> RunGet(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "status/{deviceId}")] HttpRequest req, string deviceId,
            ILogger log)
        {
            // get status of security system
            log.LogInformation("C# HTTP trigger function processed a request.");
            Dictionary<string, string> status = new Dictionary<string, string>();
            try
            {
                //via ServiceClient communiceren met iot hub
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("iotHubEdgeDeviceConnectionString"));

                //maak CloudToDeviceMethod aan en payload invullen
                CloudToDeviceMethod method = new CloudToDeviceMethod("getStatus");

                //naar device sturen
                var response = await serviceClient.InvokeDeviceMethodAsync(deviceId, "EdgeModule", method);
                var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.GetPayloadAsJson());
                log.LogInformation(jsonResponse["Status"]);
                status.Add("status", jsonResponse["Status"].ToString());
            }
            catch (Exception ex)
            {
                log.LogError("error: "+ex);
                throw;
            }
            
            return status;
        }
    }
}
