using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using logist_app.Core.Entities;
using logist_app.Models;
using logist_app.Views;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;

public partial class RoutesListViewModel : ObservableObject
{
    private ObservableCollection<Route> _routes = new();
    public List<Route> allRoutes{ get; set; } = new();
    public record AlertMessage(string Title, string Message, string Cancel);
    public ObservableCollection<Route> Routes
    {
        get => _routes;
        set { _routes = value; OnPropertyChanged(nameof(Routes)); }
    }
    [ObservableProperty]
    private string selectedFilter;

    [ObservableProperty]
    private Route selectedRoute;

    [RelayCommand]
    public async Task RefreshAsync()
    {
        allRoutes.Clear();
        await LoadRoutesAsync();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private readonly ApiSettings _api;
    private readonly IHttpClientFactory _httpFactory;

    public RoutesListViewModel(ApiSettings apiSettings, IHttpClientFactory httpClientFactory)
    {
        _api = apiSettings;
        _httpFactory = httpClientFactory;
    }

    public async Task LoadRoutesAsync()
    {
        try
        {

            if (allRoutes.Count != 0)
                return;
            var http = _httpFactory.CreateClient("Api");

            // --- ДОБАВЛЯЕМ ТОКЕН ---
            var token = await SecureStorage.Default.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            // -----------------------

            var routes = await http.GetFromJsonAsync<List<Route>>($"{_api.RoutesEndpoint}/all");

            allRoutes = routes ?? new List<Route>();
            Routes.Clear();

            if (routes is not null)
            {
                foreach (var route in allRoutes)
                    Routes.Add(route);
               
            }
            else
            {
                WeakReferenceMessenger.Default.Send(new AlertMessage("Ошибка", "Не удалось загрузить маршруты: данные отсутствуют.", "OK"));
            }
        }
        catch (Exception ex)
        {
            WeakReferenceMessenger.Default.Send(new AlertMessage("Ошибка", $"Не удалось загрузить маршруты: {ex.Message}", "OK"));
        }
    }



    //[RelayCommand]
    private async Task OpenRouteAsync(Route route)
    {
        if (route == null) return;

        Route routeToShow = route;

        if (string.IsNullOrEmpty(route.GeometryJson))
        {
            var fullRoute = await GetRouteByIdAsync(route.Id);
            if (fullRoute == null)
            {
                WeakReferenceMessenger.Default.Send(new AlertMessage("Ошибка", "Не удалось загрузить маршрут", "OK"));

                SelectedRoute = null;
                return;
            }
            routeToShow = fullRoute;
        }

        if (string.IsNullOrEmpty(routeToShow.GeometryJson))
        {
            WeakReferenceMessenger.Default.Send(new AlertMessage("Ошибка", "Маршрут не содержит данных геометрии.", "OK"));
            SelectedRoute = null;
            return;
        }
        await Shell.Current.Navigation.PushAsync(new AcceptRouteView(routeToShow.GeometryJson, routeToShow.Id));   
        SelectedRoute = null;
    }

    partial void OnSelectedRouteChanged(Route value)
    {
      
        if (value != null)
            _ = OpenRouteAsync(value);
    }

    partial void OnSelectedFilterChanged(string value)
    {

        IEnumerable<Route> query = allRoutes;

        switch (value)
        {
            case "Confirmed":
                query = query.Where(c => string.Equals(c.Status, "confirmed", StringComparison.OrdinalIgnoreCase));
                break;
            case "Draft":
                query = query.Where(c => string.Equals(c.Status, "draft", StringComparison.OrdinalIgnoreCase));
                break;
            case "Rejected":
                query = query.Where(c => string.Equals(c.Status, "rejected", StringComparison.OrdinalIgnoreCase));
                break;
            case "All":
                RestoreAll();
                break;
            default:
                RestoreAll();
                return;
        }

        Routes.Clear();
        foreach (var r in query)
            Routes.Add(r);
    }

    private void RestoreAll()
    {
        Routes.Clear();
        foreach (var r in _routes)
            Routes.Add(r);
    }

    public async Task<Route?> GetRouteByIdAsync(int id)
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

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));



    


}
