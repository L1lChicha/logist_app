using System.Text.Json.Serialization;

namespace logist_app.Core.Entities
{
    public class RouteCreationSettings
    {
        public int Id { get; set; }

        [JsonPropertyName("max_points_quantity")]
        public int MaxPointsQuantity { get; set; }

        [JsonPropertyName("min_points_quantity")]
        public int MinPointsQuantity { get; set; }

        [JsonPropertyName("max_containers_quantity")]
        public double MaxContainersQuantity { get; set; }

        [JsonPropertyName("depot_coordinates")]
        public double[] DepotCoordinates { get; set; } = new double[2];

        [JsonPropertyName("average_speed_km_h")]
        public double AverageSpeedKmH { get; set; }

        [JsonPropertyName("max_iterations2opt")]
        public int MaxIterations2Opt { get; set; }

        [JsonPropertyName("neighbor_search_radius_km")]
        public double NeighborSearchRadiusKm { get; set; }

        [JsonPropertyName("heuristic_max_speed_km_h")]
        public float HeuristicMaxSpeedKmH { get; set; }

        [JsonPropertyName("max_nearest_node_distance_km")]
        public double MaxNearestNodeDistanceKm { get; set; }

        [JsonPropertyName("truck_width_meters")]
        public double TruckWidthMeters { get; set; }

        [JsonPropertyName("truck_height_meters")]
        public double TruckHeightMeters { get; set; }

        [JsonPropertyName("min_bridge_weight_tons")]
        public double MinBridgeWeightTons { get; set; }

        [JsonPropertyName("vehicle_type")]
        public string VehicleType { get; set; } = "truck";

        [JsonPropertyName("avoid_highways")]
        public bool AvoidHighways { get; set; }

        [JsonPropertyName("use_real_speed")]
        public bool UseRealSpeed { get; set; }
    }
}