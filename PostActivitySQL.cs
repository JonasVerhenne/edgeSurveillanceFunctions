using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using CloudFunctions.Models;

namespace CloudFunctions
{
    public static class PostActivitySQL
    {
        [FunctionName("PostActivitySQL")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "activity")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string body = await new StreamReader(req.Body).ReadToEndAsync(); //body van post lezen (in json)

            //json (string body) serializeren naar object => klasse maken
            Activity activity = JsonConvert.DeserializeObject<Activity>(body);

            string newGuid = Guid.NewGuid().ToString();
            activity.Id = newGuid;
            DateTime time = DateTime.Parse(activity.Time);

            //SqlConnection: maken van connectie naar sql database server
            using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("sqlConnectionString"))) //connectionString uit local.settings.json
            {
                await connection.OpenAsync();

                //SqlCommand: opstellen van SQL statements voor de server
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection; //zeggen via welke connectie de commando's moeten gaan
                                                     //@: variabele in SQL
                    command.CommandText = "INSERT INTO Activities VALUES (@id, @time, @location, @person_detected, @image1, @image2)";
                    command.Parameters.AddWithValue("@id", activity.Id);
                    command.Parameters.AddWithValue("@time", time);
                    command.Parameters.AddWithValue("@location", activity.Location);
                    command.Parameters.AddWithValue("@person_detected", activity.PersonDetected);
                    command.Parameters.AddWithValue("@image1", activity.Image1);
                    command.Parameters.AddWithValue("@image2", activity.Image2);

                    await command.ExecuteNonQueryAsync(); //commando uitvoeren (gebruikt voor inserts)
                }
            }
            return new OkObjectResult(activity);
        }
    }
}
