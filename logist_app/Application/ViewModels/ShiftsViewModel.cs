using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using logist_app.Core.Entities;
using logist_app.Infrastructure.Service;
using logist_app.Models;
using logist_app.Views;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace logist_app.ViewModels;

public partial class ShiftsViewModel : ObservableObject
{

    private readonly IHttpClientFactory _httpClient;
    private readonly ApiSettings _api;
    [ObservableProperty]
    private ObservableCollection<Shift> shifts = new();


    public ShiftsViewModel(ApiSettings apiSettings, IHttpClientFactory httpClient)
    {
        _api = apiSettings;
        _httpClient = httpClient;
    }

    [RelayCommand]
    public async Task LoadShiftsAsync()
    {
        Shifts.Clear();
        var http = _httpClient.CreateClient("Api");

        // --- ДОБАВЛЯЕМ ТОКЕН ---
        var token = await SecureStorage.Default.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
        {
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
        // -----------------------

        var shifts = await http.GetFromJsonAsync<List<Shift>>($"{_api.RoutesEndpoint}/get-shifts");

        foreach(var s in shifts) 
        {
            Shifts.Add(s);
        }
      
    }

    [ObservableProperty]
    private Route _selectedRoute;

    async partial void OnSelectedRouteChanged(Route value)
    {
        // Если сбросили выделение в null, ничего не делаем
        if (value == null) return;

        var fullRoute = await GetRouteByIdAsync(value.Id);
        if (fullRoute == null)
        {
            await Shell.Current.DisplayAlert("Ошибка", "Не удалось загрузить маршрут", "OK");
            SelectedRoute = null; // Сбрасываем выделение при ошибке
            return;
        }

        await Shell.Current.Navigation.PushAsync(new AcceptRouteView(
            fullRoute.GeometryJson,
            fullRoute.RoutePoints,
            fullRoute.Id,
            fullRoute.Name));

        // Обязательно сбрасываем выделение в конце! 
        // Иначе, если пользователь вернется назад и кликнет на тот же маршрут, метод не сработает.
        SelectedRoute = null;
    }

    public async Task<Route?> GetRouteByIdAsync(int id)
    {
        try
        {
            var http = _httpClient.CreateClient("Api");

            // --- ДОБАВЛЯЕМ ТОКЕН ---
            var token = await SecureStorage.Default.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            // -----------------------

            var url = $"{_api.RoutesEndpoint}/{id}";

            var route = await http.GetFromJsonAsync<Route>(url, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (route == null)
            {
                //Console.WriteLine($"Маршрут с ID {id} не найден");
                // await Application.Current.MainPage.DisplayAlert("Ошибка", $"Маршрут {id} не найден", "OK");
                return null;
            }

            //Console.WriteLine($"Загружен маршрут: ID={route.Id}, Name={route.Name}, GeometryJson длина={route.geometry_json?.Length ?? 0}");
            return route;
        }
        catch (Exception ex)
        {
            //Console.WriteLine($"Ошибка загрузки маршрута {id}: {ex.Message}");
            //await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось загрузить маршрут {id}: {ex.Message}", "OK");
            return null;
        }
    }






}

