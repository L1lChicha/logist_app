using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using logist_app.Core.Entities;
using logist_app.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace logist_app.ViewModels
{
    public partial class AddVehicleViewModel : ObservableObject
    {
        private readonly ApiSettings _api;
        private readonly IHttpClientFactory _httpFactory;

        public AddVehicleViewModel(ApiSettings api, IHttpClientFactory httpFactory)
        {
            _api = api;
            _httpFactory = httpFactory;
        }

        // Поля формы
        [ObservableProperty] private string model;
        [ObservableProperty] private string licensePlate;
        [ObservableProperty] private string loadingType;
        [ObservableProperty] private string capacityStr; // Вводим как строку, парсим в double
        [ObservableProperty] private string tonnageStr;  // Вводим как строку, парсим в double

        [RelayCommand]
        private async Task Add()
        {
            if (string.IsNullOrWhiteSpace(Model) || string.IsNullOrWhiteSpace(LicensePlate))
            {
                await Shell.Current.DisplayAlert("Ошибка", "Заполните модель и номер", "OK");
                return;
            }

            // Парсинг чисел
            if (!double.TryParse(CapacityStr, out double capacity) ||
                !double.TryParse(TonnageStr, out double tonnage))
            {
                await Shell.Current.DisplayAlert("Ошибка", "Введены некорректные числа", "OK");
                return;
            }

            var newVehicle = new Vehicle
            {
                Model = Model,
                LicensePlate = LicensePlate,
                LoadingType = LoadingType,
                Capacity = capacity,
                Tonnage = tonnage,
                DistributionStatus = "not distributed", 
                CreatedAt = DateTime.UtcNow
            };

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

                var response = await http.PostAsJsonAsync($"{_api.VehiclesEndpoint}/add", newVehicle);

                if (response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert("Успех", "Транспорт добавлен", "OK");
                    // Возвращаемся назад
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Не удалось сохранить данные", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Сбой сети: {ex.Message}", "OK");
            }
        }
    }
}