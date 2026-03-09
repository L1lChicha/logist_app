using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using logist_app.Core.Entities;
using logist_app.Infrastructure.Service;
using logist_app.Models;
using Microsoft.Maui.Platform;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace logist_app.ViewModels;

public partial class AcceptRouteViewModel : ObservableObject
{
    private readonly ApiSettings _api;
    private readonly IHttpClientFactory _httpFactory;

    // Данные, которые пришли при инициализации
    private int _routeId;

    [ObservableProperty]

    private string routeName;
    private string _rawGeoJson;
    private List<RoutePoint> _routePoints;

    [ObservableProperty]
    private string newRouteName;


    [ObservableProperty]
    private ObservableCollection<RoutePoint> displayRoutePoints = new();

    // Добавляем флаг видимости списка
    [ObservableProperty]
    private bool isRoutePointsVisible = false;

    public AcceptRouteViewModel(ApiSettings api, IHttpClientFactory httpFactory)
    {
        _api = api;
        _httpFactory = httpFactory;
    }

    public void Initialize(string geoJson, List<RoutePoint> routePoint, int routeId, string routeName)
    {
        _rawGeoJson = geoJson;
        _routeId = routeId;
        _routePoints = routePoint;
        RouteName = routeName;
    }


    [RelayCommand]
    public void ShowRoutePoints()
    {
        // Переключаем видимость (чтобы кнопку можно было использовать как "Показать/Скрыть")
        IsRoutePointsVisible = !IsRoutePointsVisible;

        // Если список стал видимым, заполняем его отсортированными данными
        if (IsRoutePointsVisible)
        {
            DisplayRoutePoints.Clear();

            // Сортируем точки по SequenceNumber (правильный порядок)
            var sortedPoints = _routePoints.OrderBy(p => p.SequenceNumber).ToList();

            foreach (var point in sortedPoints)
            {
                DisplayRoutePoints.Add(point);
            }
        }
    }


    public string? PrepareJsonForDisplay()
    {
        if (string.IsNullOrWhiteSpace(_rawGeoJson)) return null;

        try
        {
            using (var doc = JsonDocument.Parse(_rawGeoJson))
            {
                var root = doc.RootElement;
                if (root.TryGetProperty("type", out var typeProp) &&
                    typeProp.GetString() == "FeatureCollection")
                {
                    if (root.TryGetProperty("features", out var features) &&
                        features.GetArrayLength() > 0)
                    {
                        var firstFeature = features[0];
                        var geometry = firstFeature.GetProperty("geometry");
                        var coordinates = geometry.GetProperty("coordinates");

                        var simpleRouteObj = new
                        {
                            type = "LineString",
                            coordinates = coordinates
                        };
                        return JsonSerializer.Serialize(simpleRouteObj);
                    }
                }
                else
                {
                    return JsonSerializer.Serialize(root);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Parsing error: {ex.Message}");
        }

        return _rawGeoJson;
    }


    


    [RelayCommand]
    public async Task ConfirmRouteAsync(string editedGeoJson)
    {
        if (string.IsNullOrWhiteSpace(NewRouteName))
        {
            await Shell.Current.DisplayAlert("Ошибка", "Введите название маршрута.", "OK");
            return;
        }

        try
        {
            var http = _httpFactory.CreateClient("Api");

            // --- ДОБАВЛЯЕМ ТОКЕН ---
            var token = await SecureStorage.Default.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            // -----------------------

            var url = $"{_api.RoutesConfirmEndpoint}/{_routeId}";

            // Если карта вернула null/пустоту (например, ничего не меняли), шлем исходный
            var jsonToSend = string.IsNullOrWhiteSpace(editedGeoJson) ? _rawGeoJson : editedGeoJson;

            // Очищаем JSON от экранирования, если оно пришло из JS неправильно
            // (В оригинальном коде этого не было, но при передаче из JS строки часто бывают лишние кавычки)
            if (jsonToSend.StartsWith("\"") && jsonToSend.EndsWith("\""))
            {
                jsonToSend = JsonSerializer.Deserialize<string>(jsonToSend);
            }
            jsonToSend = jsonToSend.Replace("\\", "");
            var payload = new
            {
                Name = NewRouteName,
                geometry_json = jsonToSend
            };

            var response = await http.PostAsJsonAsync(url, payload);

            if (response.IsSuccessStatusCode)
            {
                WeakReferenceMessenger.Default.Send(new RouteUpdatedMessage(_routeId));
                await Shell.Current.DisplayAlert("Готово", "Маршрут подтверждён!", "OK");
                await Shell.Current.Navigation.PopToRootAsync();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await Shell.Current.DisplayAlert("Ошибка", $"Сервер: {error}", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Ошибка", $"Сбой: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    public async Task RejectRouteAsync()
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");

            // --- ДОБАВЛЯЕМ ТОКЕН ---
            var token = await SecureStorage.Default.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            // -----------------------

            var url = $"{_api.RoutesRejectEndpoint}/{_routeId}";
            var response = await http.PostAsync(url, null);

            if (response.IsSuccessStatusCode)
            {
                WeakReferenceMessenger.Default.Send(new RouteUpdatedMessage(_routeId));
                await Shell.Current.DisplayAlert("Готово", "Маршрут отклонен!", "OK");
                await Shell.Current.Navigation.PopToRootAsync();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await Shell.Current.DisplayAlert("Ошибка", $"Сервер: {error}", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Ошибка", $"Сбой: {ex.Message}", "OK");
        }
    }
}