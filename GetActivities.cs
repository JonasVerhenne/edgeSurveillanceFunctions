using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using CloudFunctions.Models;
using System.Data.SqlClient;

namespace CloudFunctions
{
    public static class GetActivities
    {
        [FunctionName("GetActivities")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activity")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            List<Activity> activityList = new List<Activity>();

            //SqlConnection: maken van connectie naar sql database server
            using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("sqlConnectionString"))) //connectionString uit local.settings.json
            {
                await connection.OpenAsync();

                //SqlCommand: opstellen van SQL statements voor de server
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection; //zeggen via welke connectie de commando's moeten gaan
                    string sql = "SELECT * FROM Activities ORDER BY time DESC";
                    command.CommandText = sql; //commando toevoegen

                    //SqlDataReader: SQL statements uitvoeren en data ophalen
                    SqlDataReader reader = await command.ExecuteReaderAsync();
                    while (reader.Read()) //elke rij overlopen
                    {
                        activityList.Add(new Activity()
                        {
                            Id = reader["id"].ToString(),
                            Time = reader["time"].ToString(),
                            Location= reader["location"].ToString(),
                            PersonDetected = bool.Parse(reader["person_detected"].ToString()),
                            Image1 = reader["image1"].ToString(),
                            Image2 = reader["image2"].ToString()
                        });
                    }
                }
            }
            return new OkObjectResult(activityList);
        }

        [FunctionName("GetActivitiesByClassification")]
        public static async Task<IActionResult> RunClassify(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "activity/{classification}")] HttpRequest req, bool classification,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            List<Activity> activityList = new List<Activity>();

            //SqlConnection: maken van connectie naar sql database server
            using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("sqlConnectionString"))) //connectionString uit local.settings.json
            {
                await connection.OpenAsync();

                //SqlCommand: opstellen van SQL statements voor de server
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection; //zeggen via welke connectie de commando's moeten gaan
                    string sql = "SELECT * FROM Activities WHERE person_detected = @classification";
                    command.CommandText = sql; //commando toevoegen
                    command.Parameters.AddWithValue("@classification", classification);

                    //SqlDataReader: SQL statements uitvoeren en data ophalen
                    SqlDataReader reader = await command.ExecuteReaderAsync();
                    while (reader.Read()) //elke rij overlopen
                    {
                        activityList.Add(new Activity()
                        {
                            Id = reader["id"].ToString(),
                            Time = reader["time"].ToString(),
                            Location = reader["location"].ToString(),
                            PersonDetected = bool.Parse(reader["person_detected"].ToString()),
                            Image1 = reader["image1"].ToString(),
                            Image2 = reader["image2"].ToString()
                        });
                    }
                }
            }
            return new OkObjectResult(activityList);
        }
    }
}
