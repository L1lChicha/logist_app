using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace logist_app.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Recurrence { get; set; }
        public int ContainerCount { get; set; }
        public DateTime StartDate { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
