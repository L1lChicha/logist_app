using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace logist_app.Core.Entities
{
    public class RoutePoint
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("route_id")]
        public int RouteId { get; set; }

        [JsonIgnore]
        public Route? Route { get; set; }

        [JsonPropertyName("client_id")]
        public int ClientId { get; set; }

        [JsonPropertyName("client")]
        public Client? Client { get; set; }

        [JsonPropertyName("coordinates")]
        public string Coordinates { get; set; } = "No data";

        [JsonPropertyName("sequence_number")]
        public int SequenceNumber { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = "pending";

    }
}
