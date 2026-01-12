using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace logist_app.Core.Entities
{
    public class Vehicle
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("license_plate")] public string LicensePlate { get; set; } = "No data";
        [JsonPropertyName("model")] public string Model { get; set; } = "No data";
        [JsonPropertyName("capacity")] public double Capacity { get; set; }
        [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }
        [JsonPropertyName("loading_type")] public string LoadingType { get; set; } = "No data";
        [JsonPropertyName("tonnage")] public double Tonnage { get; set; }
        [JsonPropertyName("distribution_status")] public string? DistributionStatus { get; set; }
    }
}
