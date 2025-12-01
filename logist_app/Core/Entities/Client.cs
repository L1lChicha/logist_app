using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace logist_app.Core.Entities
{
    public class Client
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("address")] public string Address { get; set; }
        [JsonPropertyName("city")] public string City { get; set; }
        [JsonPropertyName("postal_code")] public string PostalCode { get; set; }
        [JsonPropertyName("phone")] public string Phone { get; set; }
        [JsonPropertyName("email")] public string Email { get; set; }
        [JsonPropertyName("recurrence")] public string Recurrence { get; set; }
        [JsonPropertyName("container_count")] public int ContainerCount { get; set; }
        [JsonPropertyName("start_date")] public DateTime StartDate { get; set; }
        [JsonPropertyName("coordinates")] public string Coordinates { get; set; }
        [JsonPropertyName("loading_type")] public string? LoadingType { get; set; }
        [JsonPropertyName("volume")] public double Volume { get; set; }

        [JsonPropertyName("lat")] public double Lat { get; set; }
        [JsonPropertyName("lon")] public double Lon { get; set; }
      
    }
}
