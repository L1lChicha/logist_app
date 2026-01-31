using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace logist_app.Infrastructure.Service.Dtos
{
    class RouteMonitoringDto
    {
        public int Id { get; set; }
        [JsonPropertyName("route_name")] public string RouteName { get; set; }
        [JsonPropertyName("driver_name")] public string DriverName { get; set; }
        public string Status { get; set; }
        [JsonPropertyName("total_points")] public int TotalPoints { get; set; }
        [JsonPropertyName("completed_points")] public int CompletedPoints { get; set; }
        [JsonPropertyName("start_time")] public DateTime StartTime { get; set; }
        public double Distance { get; set; }
        public double Duration { get; set; }
        [JsonPropertyName("progress_percent")] public int ProgressPercent => TotalPoints == 0 ? 0 : (int)((double)CompletedPoints / TotalPoints * 100);
        [JsonPropertyName("vehicle_name")] public string VehicleName { get; set; }
        [JsonPropertyName("loading_type")] public string LoadingType { get; set; }

    }
}
