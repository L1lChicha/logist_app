using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logist_app.Core.Entities
{
    public class RecurrenceSettings
    {
        // Тип повторения: "Daily", "Weekly", "Monthly"
        public string Type { get; set; } = "Weekly";

        // Интервал. 1 = каждая неделя/день. 2 = раз в 2 недели.
        public int Interval { get; set; } = 1;

        // Дни недели, когда вывозим. Пример: [1, 3, 5] (Пн, Ср, Пт)
        // 0 = Воскресенье, 1 = Понедельник ... 6 = Суббота (DayOfWeek enum)
        public List<DayOfWeek> DaysOfWeek { get; set; } = new();

        // Ограничение по неделям месяца (твой кейс "1-3 нед")
        // Если пусто — значит все недели. Если [1, 3] — только 1-я и 3-я.
        public List<int> WeeksOfMonth { get; set; } = new();

        // Конкретные дни месяца (для Monthly). Пример: [10, 25] (10-го и 25-го числа)
        public List<int> DaysOfMonth { get; set; } = new();
    }
}
