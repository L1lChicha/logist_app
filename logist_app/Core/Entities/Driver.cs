using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace logist_app.Core.Entities
{
    
    public class Driver
    {
            [JsonPropertyName("id")] public int Id { get; set; }
            [JsonPropertyName("name")] public string Name { get; set; } = "";
            [JsonPropertyName("phone_number")] public string PhoneNumber { get; set; } = "";

            [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }
            //[JsonIgnore] public string? PasswordHash { get; set; }  // хэш пароля (не сам пароль)

            // 🚦 Статус учетной записи
            [JsonPropertyName("is_active")] public bool IsActive { get; set; } = true;

            // 📱 Навигационное свойство к устройствам (1 -> N)
            //public ICollection<DriverDevice>? Devices { get; set; }

            // 🧾 Навигационное свойство к активационным кодам (1 -> N)
            // public ICollection<DriverActivationCode>? ActivationCodes { get; set; }

    }
}
