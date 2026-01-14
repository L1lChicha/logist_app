using System;
using System.Collections.Generic;
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
    [JsonPropertyName("points_id")] public List<int>? PointsId { get; set; }
    [JsonPropertyName("distribution_status")] public string? DistributionStatus { get; set; }


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
            // Пример: Берем исходные минуты и делим на 1000
            double result = Duration / 1000.0;

            // Если нужно делить именно ЧАСЫ, то раскомментируйте строку ниже:
            // double hours = Duration / 60.0; // 106
            // double result = hours / 100.0;  // 1.06

            return $"{result.ToString("F1", CultureInfo.InvariantCulture)} h";
        }
    
}

}
