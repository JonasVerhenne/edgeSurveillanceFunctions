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
using Microsoft.WindowsAzure.Storage.Blob;

namespace CloudFunctions
{
    public static class PostImageBlob
    {
        [FunctionName("PostImageBlob")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "image/{fileName}")] HttpRequest req, string fileName,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            fileName = fileName+".png";
            CloudStorageAccount account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("blobStorageConnectionString"));// connectionstring
            CloudBlobClient client = account.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference("images");// container name
            CloudBlockBlob blob = container.GetBlockBlobReference(fileName);

            await blob.UploadFromStreamAsync(req.Body);

            return new OkObjectResult(200);
        }
    }
}
