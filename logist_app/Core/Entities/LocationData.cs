using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace logist_app.Core.Entities
{
        public class LocationData
        {
            [JsonPropertyName("device_id")]
            public string DeviceId { get; set; }
            public double Lat { get; set; }
            public double Lon { get; set; }
            public double Speed { get; set; }
            public long Timestamp { get; set; }

            [JsonPropertyName("driver_name")]
            public string DriverName { get; set; }

            [JsonPropertyName("license_plate")]
            public string LicensePlate { get; set; }
        }
}
