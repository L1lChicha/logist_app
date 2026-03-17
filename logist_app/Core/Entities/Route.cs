using logist_app.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
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
    [JsonPropertyName("is_distributed")] public bool IsDistributed { get; set; }
    [JsonPropertyName("vehicle_loading_type")] public string? VehicleLoadingType { get; set; }
    [JsonPropertyName("route_points")] public List<RoutePoint> RoutePoints { get; set; } = new();
    [Column(TypeName = "jsonb")]
    [JsonPropertyName("statistics")] public StatisticsData Statistics { get; set; }

    public string DisplayTonnage
    {
        get
        {
           return $"{Statistics.TotalTonnage.ToString("F1", CultureInfo.InvariantCulture)} T";

        }
    }

    public string DisplayVolume
    {
        get
        {
            return $"{Statistics.TotalVolume.ToString("F1", CultureInfo.InvariantCulture)} М³";

        }
    }


    public string DisplayDistance
    {
        get
        {
            // Делим на 1000, чтобы сдвинуть запятую на 3 знака влево
            double result = Distance / 1000.0;

            // "F1" - один знак после точки, "F2" - два знака
            return $"{result.ToString("F1", CultureInfo.InvariantCulture)} km";
        }
    }

    // 2. ВРЕМЯ
    // Задача: Оставить 1 цифру в начале 
    // Вариант А: Если считаем от минут (6359 -> 6.3) - делим на 1000
    // Вариант Б: Если считаем от часов (106 -> 1.06) - делим часы на 100
    public string DisplayDuration
    {
        get
        {
            var time = TimeSpan.FromSeconds(Duration);

            // Форматируем строку: если есть часы, выводим с часами, если только минуты — без часов
            if (time.Hours > 0)
            {
                return $"{time.Hours} ч {time.Minutes} мин";
            }

            return $"{time.Minutes} мин";
        }
    
    }
    public class StatisticsData
    {
        [JsonPropertyName("total_volume")] public double TotalVolume { get; set; }
        [JsonPropertyName("total_tonnage")] public double TotalTonnage { get; set; }
        [JsonPropertyName("big_container_quantity")] public int BigContainerQuantity { get; set; }
        [JsonPropertyName("standard_container_quantity")] public int StandardContainerQuantity { get; set; }
    }

}
