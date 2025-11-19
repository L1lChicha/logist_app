
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using logist_app.Models;
using logist_app.Views;
using Microsoft.Maui.Controls;
using QRCoder;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Xml.Linq;


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

        [ObservableProperty]
        private Driver _selectedDriver;

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

        [ObservableProperty]
        string code;

       

        [RelayCommand]
        public void ShowCode(string code)
        {
            Code = code;
        }

        [ObservableProperty]
        private ImageSource qrImage;


        [RelayCommand]
        public async Task GetAuthorizationCode()
        {

            var id = _selectedDriver.Id;
            int codeLength = 10;
            int codeHoursValid = 1;

            var http = _httpFactory.CreateClient("Api");
            var url = $"{_api.DriverGetCodeUrl}/{id}";
            var response = await http.PostAsJsonAsync(url, new { codeLength , codeHoursValid });
            var json = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            ShowCode(json["code"].ToString());


            var generator = new QRCodeGenerator();
            var data = generator.CreateQrCode(json["code"].ToString(), QRCodeGenerator.ECCLevel.Q);

            var png = new PngByteQRCode(data);
            byte[] qrBytes = png.GetGraphic(20); // масштаб QR — 20

            QrImage = ImageSource.FromStream(() => new MemoryStream(qrBytes));
        }


       
    }
}
