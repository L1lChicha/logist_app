using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using logist_app.Core.Entities;
using logist_app.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace logist_app.ViewModels
{
    public partial class RouteSettingsViewModel : ObservableObject
    {
        private readonly ApiSettings _api;
        private readonly IHttpClientFactory _httpFactory;

        // --- Список типов транспорта для Picker ---
        public ObservableCollection<string> VehicleTypes { get; } = new() { "truck", "car", "foot" };

        public RouteSettingsViewModel(ApiSettings apiSettings, IHttpClientFactory httpClientFactory)
        {
            _api = apiSettings;
            _httpFactory = httpClientFactory;
            LoadSettingsCommand.Execute(null);
        }

        // === Свойства (генерируются автоматически библиотекой MVVM Toolkit) ===

        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private int id;

        // Логистика
        [ObservableProperty] private int maxPointsQuantity;
        [ObservableProperty] private int minPointsQuantity;
        [ObservableProperty] private double maxContainersQuantity;
        [ObservableProperty] private double depotLat;
        [ObservableProperty] private double depotLon;
        [ObservableProperty] private double averageSpeedKmh;

        // Алгоритм
        [ObservableProperty] private int maxIterations2Opt;
        [ObservableProperty] private double neighborSearchRadiusKm;
        [ObservableProperty] private float heuristicMaxSpeedKmH;
        [ObservableProperty] private double maxNearestNodeDistanceKm;

        // Транспорт
        [ObservableProperty] private double truckWidthMeters;
        [ObservableProperty] private double truckHeightMeters;
        [ObservableProperty] private double minBridgeWeightTons;
        [ObservableProperty] private string selectedVehicleType;
        [ObservableProperty] private bool avoidHighways;
        [ObservableProperty] private bool useRealSpeed;

        // === Команды ===

        [RelayCommand]
        private async Task LoadSettingsAsync()
        {
            IsLoading = true;
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
                else
                {
                    // Если токена нет, можно выкинуть на логин или показать ошибку
                    await Shell.Current.DisplayAlert("Ошибка", "Вы не авторизованы", "OK");
                    return;
                }
                // -----------------------

                var settings = await http.GetFromJsonAsync<RouteCreationSettings>($"{_api.RoutingSettingsEndpoint}");

                if (settings != null)
                {
                    Id = settings.Id;
                    MaxPointsQuantity = settings.MaxPointsQuantity;
                    MinPointsQuantity = settings.MinPointsQuantity;
                    MaxContainersQuantity = settings.MaxContainersQuantity;

                    // Разбираем массив координат
                    if (settings.DepotCoordinates?.Length >= 2)
                    {
                        DepotLat = settings.DepotCoordinates[0];
                        DepotLon = settings.DepotCoordinates[1];
                    }

                    AverageSpeedKmh = settings.AverageSpeedKmH;
                    MaxIterations2Opt = settings.MaxIterations2Opt;
                    NeighborSearchRadiusKm = settings.NeighborSearchRadiusKm;
                    HeuristicMaxSpeedKmH = settings.HeuristicMaxSpeedKmH;
                    MaxNearestNodeDistanceKm = settings.MaxNearestNodeDistanceKm;
                    TruckWidthMeters = settings.TruckWidthMeters;
                    TruckHeightMeters = settings.TruckHeightMeters;
                    MinBridgeWeightTons = settings.MinBridgeWeightTons;
                    SelectedVehicleType = settings.VehicleType;
                    AvoidHighways = settings.AvoidHighways;
                    UseRealSpeed = settings.UseRealSpeed;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось загрузить настройки: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task SaveSettingsAsync()
        {
            IsLoading = true;
            try
            {
                // Собираем объект для отправки
                var settingsToSend = new RouteCreationSettings
                {
                    Id = this.Id, // Обычно 1
                    MaxPointsQuantity = this.MaxPointsQuantity,
                    MinPointsQuantity = this.MinPointsQuantity,
                    MaxContainersQuantity = this.MaxContainersQuantity,
                    DepotCoordinates = new double[] { this.DepotLat, this.DepotLon }, // Собираем массив обратно
                    AverageSpeedKmH = this.AverageSpeedKmh,
                    MaxIterations2Opt = this.MaxIterations2Opt,
                    NeighborSearchRadiusKm = this.NeighborSearchRadiusKm,
                    HeuristicMaxSpeedKmH = this.HeuristicMaxSpeedKmH,
                    MaxNearestNodeDistanceKm = this.MaxNearestNodeDistanceKm,
                    TruckWidthMeters = this.TruckWidthMeters,
                    TruckHeightMeters = this.TruckHeightMeters,
                    MinBridgeWeightTons = this.MinBridgeWeightTons,
                    VehicleType = this.SelectedVehicleType,
                    AvoidHighways = this.AvoidHighways,
                    UseRealSpeed = this.UseRealSpeed
                };

                var http = _httpFactory.CreateClient("Api");

                // --- ДОБАВЛЯЕМ ТОКЕН ---
                var token = await SecureStorage.Default.GetAsync("auth_token");
                if (!string.IsNullOrEmpty(token))
                {
                    http.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }
                // -----------------------

                var response = await http.PutAsJsonAsync($"{_api.RoutingSettingsEndpoint}", settingsToSend);

                if (response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert("Успех", "Настройки сохранены!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Сервер вернул ошибку при сохранении.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось сохранить: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}