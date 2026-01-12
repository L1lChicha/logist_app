using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using logist_app.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace logist_app.ViewModels;

public partial class AcceptRouteViewModel : ObservableObject
{
    private readonly ApiSettings _api;
    private readonly IHttpClientFactory _httpFactory;

    // Данные, которые пришли при инициализации
    private int _routeId;
    private string _rawGeoJson;

    [ObservableProperty]
    private string routeName;

    public AcceptRouteViewModel(ApiSettings api, IHttpClientFactory httpFactory)
    {
        _api = api;
        _httpFactory = httpFactory;
    }

    public void Initialize(string geoJson, int routeId)
    {
        _rawGeoJson = geoJson;
        _routeId = routeId;
    }

    /// <summary>
    /// Логика подготовки JSON для отображения на карте.
    /// Перенесена из View, так как это обработка данных.
    /// </summary>
    public string PrepareJsonForDisplay()
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
            Console.WriteLine($"Ошибка парсинга: {ex.Message}");
        }

        // Возвращаем исходный, если не удалось упростить, или он уже в нужном формате
        return _rawGeoJson;
    }

    [RelayCommand]
    public async Task ConfirmRouteAsync(string editedGeoJson)
    {
        if (string.IsNullOrWhiteSpace(RouteName))
        {
            await Shell.Current.DisplayAlert("Ошибка", "Введите название маршрута.", "OK");
            return;
        }

        try
        {
            var http = _httpFactory.CreateClient("Api");
            var url = $"{_api.RoutesConfirmEndpoint}/{_routeId}";

            // Если карта вернула null/пустоту (например, ничего не меняли), шлем исходный
            var jsonToSend = string.IsNullOrWhiteSpace(editedGeoJson) ? _rawGeoJson : editedGeoJson;

            // Очищаем JSON от экранирования, если оно пришло из JS неправильно
            // (В оригинальном коде этого не было, но при передаче из JS строки часто бывают лишние кавычки)
            if (jsonToSend.StartsWith("\"") && jsonToSend.EndsWith("\""))
            {
                jsonToSend = JsonSerializer.Deserialize<string>(jsonToSend);
            }

            var payload = new
            {
                Name = RouteName,
                geometry_json = jsonToSend
            };

            var response = await http.PostAsJsonAsync(url, payload);

            if (response.IsSuccessStatusCode)
            {
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
            var url = $"{_api.RoutesRejectEndpoint}/{_routeId}";
            var response = await http.PostAsync(url, null);

            if (response.IsSuccessStatusCode)
            {
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