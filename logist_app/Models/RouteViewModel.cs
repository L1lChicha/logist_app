using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


    public class RouteViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }
        public double Distance { get; set; }
        public double Duration { get; set; }
        [JsonPropertyName("geometry_json")] public string GeometryJson { get; set; }
    }
