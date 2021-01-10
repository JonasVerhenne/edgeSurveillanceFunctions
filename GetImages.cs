using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace CloudFunctions
{
    public static class GetImages
    {
        [FunctionName("GetImages")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "image/{id}")] HttpRequest req, string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            var account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("blobStorageConnectionString"));//connectionstring
            log.LogInformation("connected to blob");
            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference("images");//container name
            var blob = container.GetBlockBlobReference(id);//name of image
            log.LogInformation("found image");

            var stream = new MemoryStream();
            await blob.DownloadToStreamAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return result;
        }
    }
}
