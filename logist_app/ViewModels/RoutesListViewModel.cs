using CommunityToolkit.Mvvm.ComponentModel;
using logist_app.Models;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http.Json;

public partial class RoutesListViewModel : ObservableObject
{
    private ObservableCollection<Route> _routes = new();
    public List<Route> allRoutes{ get; set; } = new();


    [ObservableProperty]
    private string selectedItem;

    public ObservableCollection<Route> Routes
    {
        get => _routes;
        set { _routes = value; OnPropertyChanged(nameof(Routes)); }
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
            var http = _httpFactory.CreateClient("Api"); // BaseUrl уже задан в AddHttpClient
            // "Route/all" — используем относительный путь относительно BaseUrl
            var routes = await http.GetFromJsonAsync<List<Route>>($"{_api.RoutesEndpoint}/all");

            Routes.Clear();

            if (routes is not null)
            {
                foreach (var route in routes)
                {
                    Routes.Add(route);
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Не удалось загрузить маршруты: данные отсутствуют.", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки маршрутов: {ex.Message}");
            await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось загрузить маршруты: {ex.Message}", "OK");
        }
    }


    partial void OnSelectedItemChanged(string value)
    {
        string sortParam = value;
        //IEnumerable<Client> query = (IEnumerable<Client>)allRoutes;
        switch (sortParam)
        {
            case "Confirmed":
          
                var sorted = allRoutes.Where(c => c.CreatedBy == "Confirmed").ToList();
                allRoutes.Clear();
                allRoutes.AddRange(sorted);
                break;
        }
    }

    public async Task<Route?> GetRouteByIdAsync(int id)
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
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
