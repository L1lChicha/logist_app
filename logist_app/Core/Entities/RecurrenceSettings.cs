using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace logist_app.Core.Entities
{
    public class RecurrenceSettings
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "Weekly";

        // Интервал. 1 = каждая неделя/день. 2 = раз в 2 недели.
        [JsonPropertyName("interval")]
        public int Interval { get; set; } = 1;

        // Дни недели, когда вывозим. Пример: [1, 3, 5] (Пн, Ср, Пт)
        // 0 = Воскресенье, 1 = Понедельник ... 6 = Суббота (DayOfWeek enum)
        [JsonPropertyName("days_of_week")]
        public List<DayOfWeek> DaysOfWeek { get; set; } = new();

        // Ограничение по неделям месяца (твой кейс "1-3 нед")
        // Если пусто — значит все недели. Если [1, 3] — только 1-я и 3-я.
        [JsonPropertyName("weeks_of_month")]

        public List<int> WeeksOfMonth { get; set; } = new();

        // Конкретные дни месяца (для Monthly). Пример: [10, 25] (10-го и 25-го числа)
        [JsonPropertyName("days_of_month")]

        public List<int> DaysOfMonth { get; set; } = new();
    }
}
