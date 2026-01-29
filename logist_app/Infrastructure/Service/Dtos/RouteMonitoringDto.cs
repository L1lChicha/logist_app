using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logist_app.Infrastructure.Service.Dtos
{
    class RouteMonitoringDto
    {
        public int Id { get; set; }
        public string RouteName { get; set; }
        public string DriverName { get; set; }
        public string Status { get; set; }
        public int TotalPoints { get; set; }
        public int CompletedPoints { get; set; }
        public DateTime StartTime { get; set; }
        public int ProgressPercent => TotalPoints == 0 ? 0 : (int)((double)CompletedPoints / TotalPoints * 100);
    }
}
