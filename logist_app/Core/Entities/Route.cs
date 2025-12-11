using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


public class Route
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }
    [JsonPropertyName("distance")] public double Distance { get; set; }
    [JsonPropertyName("duration")] public double Duration { get; set; }
    [JsonPropertyName("geometry_json")] public string? GeometryJson { get; set; }
    [JsonPropertyName("status")] public string? Status { get; set; }
    [JsonPropertyName("created_by")] public string? CreatedBy { get; set; }
    [JsonPropertyName("points_id")] public List<int>? PointsId { get; set; }
    [JsonPropertyName("distribution_status")] public string? DistributionStatus { get; set; }


}
