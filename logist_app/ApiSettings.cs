namespace logist_app.Models
{
    public class ApiSettings
    {
        public string BaseUrl { get; set; }
        public string ClientsEndpoint { get; set; }

        // --- Маршруты ---
        public string RoutesEndpoint { get; set; }
        public string RoutesBuildEndpoint { get; set; }
        public string RoutesConfirmEndpoint { get; set; }
        public string RoutesRejectEndpoint { get; set; }
        public string DriversEndpoint { get; set; }
        public string DriverGetCodeEndpoint { get; set; }

        // --- Полные URL ---
        public string ClientsUrl => $"{BaseUrl}{ClientsEndpoint}";
        public string RoutesUrl => $"{BaseUrl}{RoutesEndpoint}";
        public string DriversUrl => $"{BaseUrl}{DriversEndpoint}";
        public string RoutesBuildUrl => $"{BaseUrl}{RoutesBuildEndpoint}";
        public string RoutesConfirmUrl => $"{BaseUrl}{RoutesConfirmEndpoint}";
        public string RoutesRejectUrl => $"{BaseUrl}{RoutesRejectEndpoint}";
        public string DriverGetCodeUrl => $"{BaseUrl}{DriverGetCodeEndpoint}";


    }
}