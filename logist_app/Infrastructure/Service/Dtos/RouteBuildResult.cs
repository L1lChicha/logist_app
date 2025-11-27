using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace logist_app.Infrastructure.Service.Dtos
{
    public class RouteBuildResult
    {
        
        [JsonPropertyName("route_id")]
        public int RouteId { get; set; }

        [JsonPropertyName("route_data")]
        public string RouteData { get; set; }

        public string Response { get; set; }
        public class ApiError
        {
            public string Message { get; set; }
        }
    }
}
