using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudFunctions.Models
{
    class Activity
    {
        public string Id { get; set; }
        public DateTime Time { get; set; }
        public string Location { get; set; }
        [JsonProperty(PropertyName = "person_detected")]
        public bool PersonDetected { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
    }
}
