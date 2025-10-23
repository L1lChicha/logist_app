
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using logist_app.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace logist_app.ViewModels
{
    public partial class DriversViewModel : ObservableObject
    {
        private readonly ApiSettings _api;
        private readonly IHttpClientFactory _httpFactory;

        public DriversViewModel(ApiSettings apiSettings, IHttpClientFactory httpClientFactory)
        {
            _api = apiSettings;
            _httpFactory = httpClientFactory;
        }

        [ObservableProperty]
        public ObservableCollection<Driver> drivers = new();
        
        [RelayCommand]

        public async Task LoadDriversAsync()
        {
            try
            {
                var http = _httpFactory.CreateClient("Api");
                var drivers = await http.GetFromJsonAsync<List<Driver>>($"{_api.DriversEndpoint}/all");
                Drivers.Clear();

                if (drivers is not null)
                {
                    foreach (var driver in drivers)
                    {
                        Drivers.Add(driver);
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Не удалось загрузить маршруты: данные отсутствуют.", "OK");
                }
            }
            catch (Exception ex) 
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Не удалось загрузить маршруты: данные отсутствуют. {ex.Message}", "OK");

            }
        }

        
    }
}
