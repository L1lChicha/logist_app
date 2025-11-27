using logist_app.Core.Entities;
using logist_app.Core.Interfaces;
using logist_app.Infrastructure.Service.Dtos;
using logist_app.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace logist_app.Infrastructure.Service
{
    public class RouteService : IRouteService
    {
        private readonly IHttpClientFactory httpFactory;
        private readonly ApiSettings api;

        public RouteService(IHttpClientFactory _httpClientFactory, ApiSettings _apiSettings)
        {
            httpFactory = _httpClientFactory;
            api = _apiSettings;
        }

        public async Task<RouteBuildResult> BuildRoute(List<int> clientsId)
        {
            try
            {
                var http = httpFactory.CreateClient("Api");
                var response = await http.PostAsJsonAsync(api.RoutesBuildEndpoint, clientsId);

                if (response.IsSuccessStatusCode)
                {
                    using var stream = await response.Content.ReadAsStreamAsync();
                    using var json = await JsonDocument.ParseAsync(stream);

                    var routeDataElement = json.RootElement.GetProperty("route_data");
                    var routeDataJson = routeDataElement.GetRawText();

                    var routeId = json.RootElement.GetProperty("route_id").GetInt32();

                   // var routeData = json.RootElement.GetProperty("route_data").GetString();
                   

                    return new RouteBuildResult
                    {
                     //   RouteData = routeData,
                        RouteId = routeId,
                        Response = response.StatusCode.ToString()
                    };
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return new RouteBuildResult
                    {
                        Response = $"Server returned {(int)response.StatusCode}: {error}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new RouteBuildResult
                {
                   
                    Response = ex.Message
                };
            }
        }

    }
}
