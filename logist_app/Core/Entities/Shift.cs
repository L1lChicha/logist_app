using Microsoft.Maui.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace logist_app.Core.Entities
{
    public class Shift
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("routes_id")] public List<Route> RoutesId { get; set; }
        [JsonPropertyName("vehicle_id")] public int VehicleId { get; set; }
        [JsonPropertyName("appointed_date")] public DateTime AppointedDate { get; set; }
        [JsonPropertyName("driver_id")] public int DriverId { get; set; }
        [JsonPropertyName("total_duration")] public double TotalDuration { get; set; }
        [JsonPropertyName("total_distance")] public double TotalDistance { get; set; }


        [JsonIgnore]
        public string DisplayDistance
        {
            get
            {
                // Делим метры на 1000 для километров и форматируем до 1 знака (F1)
                return $"{(TotalDistance / 1000):F1} км";
            }
        }

        [JsonIgnore]
        public string DisplayDuration
        {
            get
            {
                // Переводим секунды в удобный объект TimeSpan
                TimeSpan time = TimeSpan.FromSeconds(TotalDuration);

                // Если нужно в формате "02:29" (часы:минуты)
                // Исходное время может быть больше 24 часов, поэтому берем TotalHours
                int hours = (int)time.TotalHours;
                int minutes = time.Minutes;

                return $"{hours:D2}:{minutes:D2}";

                // Альтернатива текстом: return $"{hours} ч {minutes} мин";
            }
        }

    }
}
