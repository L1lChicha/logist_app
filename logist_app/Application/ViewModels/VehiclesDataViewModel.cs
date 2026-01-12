using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using logist_app.Core.Entities;
using logist_app.Infrastructure.Service;
using logist_app.Models;
using logist_app.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace logist_app.ViewModels
{
    public partial class VehiclesDataViewModel : ObservableObject
    {


        // Коллекция для привязки
        public ObservableCollection<Vehicle> Vehicles { get; } = new();
        private readonly ApiSettings _api;
        private readonly IHttpClientFactory _httpFactory;


        public VehiclesDataViewModel(ApiSettings apiSettings, IHttpClientFactory httpClientFactory)
        {
            _api = apiSettings;
            _httpFactory = httpClientFactory;
        }


        // Выбранный элемент
        [ObservableProperty]
        private Vehicle _selectedVehicle;




        [RelayCommand]
        public async Task LoadVehiclesAsync()
        {

            try
            {
                if (Vehicles.Count != 0)
                    return;
                var http = _httpFactory.CreateClient("Api");
                var vehicles = await http.GetFromJsonAsync<List<Vehicle>>($"{_api.VehiclesEndpoint}");
                Vehicles.Clear();

                if (vehicles is not null)
                {
                    foreach (var vehicle in vehicles)
                    {
                        Vehicles.Add(vehicle);
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Не удалось загрузить транспорт: данные отсутствуют.", "OK");

                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Не удалось загрузить транспорт: данные отсутствуют. {ex.Message}", "OK");

            }

        }
        [RelayCommand]
        private async Task AddVehicle()
        {
            var page = App.Services.GetService<AddVehicleView>();

            await Shell.Current.Navigation.PushAsync(page);
        }


        [RelayCommand]

        private async Task Delete()
        {
            bool confirm = await Shell.Current.DisplayAlert("Confirm", $"Delete {_selectedVehicle.LicensePlate}?", "Yes", "No");
            if (!confirm) return;

            var http = _httpFactory.CreateClient("Api");
            var response = await http.DeleteAsync($"{_api.VehiclesEndpoint}/{_selectedVehicle.Id}");

            if (response.IsSuccessStatusCode)
            {
                await Shell.Current.DisplayAlert("Deleted", "Vehicle deleted.", "OK");

            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await Shell.Current.DisplayAlert("Error", $"Failed to delete Vehicle.\n{error}", "OK");
            }
        }

        // Команда обновления
        [RelayCommand]
        private async Task Refresh()
        {
            Vehicles.Clear();

            await LoadVehiclesAsync();
        }




        [RelayCommand]
        private void ToggleSelection(Vehicle vehicle)
        {
            if (vehicle == null) return;

            // Если нажали на уже выбранный элемент -> Сбрасываем выбор
            if (SelectedVehicle == vehicle)
            {
                SelectedVehicle = null;
            }
            else
            {
                // Иначе -> Выбираем новый
                SelectedVehicle = vehicle;
            }
        }
    }
}
