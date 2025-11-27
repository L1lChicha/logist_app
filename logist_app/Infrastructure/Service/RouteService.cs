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

        //public async Task<RouteBuildResult> BuildRoute()
        //{
        //    try
        //    {
        //        var http = httpFactory.CreateClient("Api");
        //        var response = await http.PostAsJsonAsync(api.RoutesBuildEndpoint, selectedClientIds);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            using var stream = await response.Content.ReadAsStreamAsync();
        //            using var json = await JsonDocument.ParseAsync(stream);

        //            var routeData = json.RootElement.GetProperty("route_data").ToString();
        //            var routeId = int.Parse(json.RootElement.GetProperty("route_id").ToString());

        //            //await DisplayAlert("Success", "Маршрут успешно построен", "OK");
        //            //await Navigation.PushAsync(new AcceptRouteView(routeData, routeId));

                   
        //        }
        //        else
        //        {
        //            var error = await response.Content.ReadAsStringAsync();
        //          //  await DisplayAlert("Error", $"{response.StatusCode}\n{error}", "OK");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //       // await DisplayAlert("Error", ex.Message, "OK");
        //    }

        //}
    }
}
