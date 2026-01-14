
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using logist_app.Core.Entities;
using logist_app.Models;
using logist_app.Views;
using Microsoft.Maui.Controls;
using QRCoder;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
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

        [ObservableProperty]
        private bool isButtonEnabled = false;


        partial void OnSelectedDriverChanged(Driver value)
        {
            IsButtonEnabled = value != null;
        }


        [RelayCommand]
        public async Task LoadDriversAsync()
        {

            if (Drivers.Count != 0)
                return;
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
                    await Shell.Current.DisplayAlert("Error", "Не удалось загрузить маршруты: данные отсутствуют.", "OK");

                }
            }
            catch (Exception ex) 
            {
                await Shell.Current.DisplayAlert("Error", $"Не удалось загрузить водителей: данные отсутствуют. {ex.Message}", "OK");

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

            try
            {
                var id = _selectedDriver.Id;
                int codeLength = 10;
                int codeHoursValid = 1;

                var http = _httpFactory.CreateClient("Api");

                // --- ДОБАВЛЯЕМ ТОКЕН ---
                var token = await SecureStorage.Default.GetAsync("auth_token");
                if (!string.IsNullOrEmpty(token))
                {
                    http.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }
                // -----------------------

                var url = $"{_api.DriverGetCodeUrl}/{id}";

                // 1. Отправляем запрос
                var response = await http.PostAsJsonAsync(url, new { codeLength, codeHoursValid });

                // 2. Проверяем статус (200-299)
                if (response.IsSuccessStatusCode)
                {
                    // Успех! Читаем код
                    var json = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

                    if (json != null && json.ContainsKey("code"))
                    {
                        ShowCode(json["code"].ToString() ?? "No code");
                        var generator = new QRCodeGenerator();
                        var data = generator.CreateQrCode(json["code"].ToString() ?? "No code", QRCodeGenerator.ECCLevel.Q);

                        var png = new PngByteQRCode(data);
                        byte[] qrBytes = png.GetGraphic(20); // масштаб QR — 20

                        QrImage = ImageSource.FromStream(() => new MemoryStream(qrBytes));
                    }
                    else
                    {
                        //await Application.Current.MainPage.DisplayAlert("Ошибка", "Сервер вернул пустой ответ", "OK");
                        await Shell.Current.DisplayAlert("Ошибка", "Сервер вернул пустой ответ", "OK");
                    }
                }
                else
                {
                   
                    string errorMessage = "Неизвестная ошибка сервера";
                    try
                    {
                        // Часто сервер шлет JSON вида { "message": "Текст ошибки" } или "title"
                        var errorJson = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                        if (errorJson != null && errorJson.ContainsKey("message"))
                        {
                            errorMessage = errorJson["message"]?.ToString() ?? "Неизвестная ошибка";
                        }
                    }
                    catch
                    {
                        // Если сервер прислал не JSON, а просто текст или HTML (например, nginx 502 error)
                        errorMessage = await response.Content.ReadAsStringAsync();
                    }
                    await Shell.Current.DisplayAlert("Ошибка запроса",
                        $"Код: {response.StatusCode}\nДетали: {errorMessage}", "OK");
             
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка сети",
                    $"Не удалось связаться с сервером.\n{ex.Message}", "OK");
              
            }


           
        }


       
    }
}
