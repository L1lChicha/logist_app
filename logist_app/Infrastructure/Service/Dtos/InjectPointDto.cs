using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace logist_app.Infrastructure.Service.Dtos
{
    public class InjectPointDto
    {
        // Если добавляем существующего
        public int? ExistingClientId { get; set; }

        // Если создаем нового
        public string? NewClientName { get; set; }
        public string? NewClientAddress { get; set; }
        public double? NewClientLat { get; set; }
        public double? NewClientLon { get; set; }
        [JsonPropertyName("container_count")]
        public int ContainerCount { get; set; } = 1;
    }
}
