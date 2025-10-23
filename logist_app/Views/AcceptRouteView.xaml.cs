using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using logist_app.Models;

namespace logist_app.Views;

public partial class AcceptRouteView : ContentPage
{
    private readonly string _routeGeoJson;
    private readonly int _routeId;

    // DI-зависимости (берём из контейнера в конструкторе)
    private readonly ApiSettings _api;
    private readonly IHttpClientFactory _httpFactory;

    public AcceptRouteView(string routeGeoJson, int routeId)
    {
        InitializeComponent();

        if (string.IsNullOrEmpty(routeGeoJson))
            throw new ArgumentNullException(nameof(routeGeoJson), "RouteGeoJson не может быть null или пустым");
        if (routeId <= 0)
            throw new ArgumentException("RouteId должен быть положительным числом", nameof(routeId));

        _routeGeoJson = routeGeoJson;
        _routeId = routeId;

        _api = App.Services.GetRequiredService<ApiSettings>();
        _httpFactory = App.Services.GetRequiredService<IHttpClientFactory>();

       
    }

    private async void MapWebView_Navigated(object sender, WebNavigatedEventArgs e)
    {
        try
        {
            await MapWebView.EvaluateJavaScriptAsync(
                $"displayRoute('{_routeGeoJson.Replace("'", "\\'")}')");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось отобразить маршрут: {ex.Message}", "OK");
        }
    }

    private async void ShowRouteButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            string jsCode = $"displayRoute('{_routeGeoJson.Replace("'", "\\'").Replace("\n", "")}');";
            await MapWebView.EvaluateJavaScriptAsync(jsCode);
            await MapWebView.EvaluateJavaScriptAsync("map.invalidateSize();");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось отобразить маршрут: {ex.Message}", "OK");
        }
    }

    private async void confirmRoute_Clicked(object sender, EventArgs e)
    {
        try
        {
            var name = RouteNameEntry.Text;

            if (string.IsNullOrWhiteSpace(name))
            {
                await DisplayAlert("Ошибка", "Введите название маршрута.", "OK");
                return;
            }

            var http = _httpFactory.CreateClient("Api");
            var url = $"{_api.RoutesConfirmEndpoint}/{_routeId}";

            
         
            var response = await http.PostAsJsonAsync(url, new { name });

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Готово", "Маршрут подтверждён!", "OK");
                await Navigation.PopToRootAsync();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Ошибка", $"Сервер вернул ошибку: {error}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось выполнить запрос: {ex.Message}", "OK");
        }
    }


    private async void rejectRoute_Clicked(object sender, EventArgs e)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var url = $"{_api.RoutesRejectEndpoint}/{_routeId}";
            var response = await http.PostAsync(url, null);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Готово", "Маршрут отклонен!", "OK");
                await Navigation.PopToRootAsync();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Ошибка", $"Сервер вернул ошибку: {error}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось выполнить запрос: {ex.Message}", "OK");
        }
    }
}
