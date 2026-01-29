using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace logist_app.Core.Entities
{
    public class RoutePoint
    {
        public int Id { get; set; }

        // Внешний ключ на Маршрут
        public int RouteId { get; set; }
        [JsonIgnore] // Чтобы не зацикливало при сериализации
        public Route? Route { get; set; }

        // Внешний ключ на Клиента
        public int ClientId { get; set; }
        public Client? Client { get; set; }

        // Порядковый номер точки в маршруте
        public int SequenceNumber { get; set; }

        // Статус точки (pending, visited, skipped)
        public string Status { get; set; } = "pending";
    }
}
