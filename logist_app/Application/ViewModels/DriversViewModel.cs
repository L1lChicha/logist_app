
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using logist_app.Core.Entities;
using logist_app.Models;
using logist_app.Views;
using Microsoft.Maui.Controls;
using Microsoft.VisualBasic.FileIO;
using QRCoder;
using System.Collections.ObjectModel;
using System.Data;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;


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
                double codeHoursValid = 1;

                var http = _httpFactory.CreateClient("Api");

                var token = await SecureStorage.Default.GetAsync("auth_token");
                if (!string.IsNullOrEmpty(token))
                {
                    http.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var url = $"{_api.DriverGetCodeUrl}/{id}?hoursValid={codeHoursValid}";

                var response = await http.PostAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

                    if (json != null && json.ContainsKey("code"))
                    {
                        ShowCode(json["code"].ToString() ?? "No code");
                        var generator = new QRCodeGenerator();
                        var data = generator.CreateQrCode(json["code"].ToString() ?? "No code", QRCodeGenerator.ECCLevel.Q);

                        var png = new PngByteQRCode(data);
                        byte[] qrBytes = png.GetGraphic(20); 

                        QrImage = ImageSource.FromStream(() => new MemoryStream(qrBytes));
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "The server returned an empty response.", "OK");
                    }
                }
                else
                {
                   
                    string errorMessage = "Неизвестная ошибка сервера";
                    try
                    {
                        var errorJson = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                        if (errorJson != null && errorJson.ContainsKey("message"))
                        {
                            errorMessage = errorJson["message"]?.ToString() ?? "Неизвестная ошибка";
                        }
                    }
                    catch
                    {
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


        [RelayCommand]
        public async Task Delete()
        {
            // 0. Проверка на null (если водитель не выбран)
            if (SelectedDriver == null)
            {
                await Shell.Current.DisplayAlert("Ошибка", "Выберите водителя для удаления", "OK");
                return;
            }

            // 1. Спрашиваем подтверждение
            bool confirm = await Shell.Current.DisplayAlert(
                "Подтверждение",
                $"Вы действительно хотите удалить водителя {SelectedDriver.Name}?\nЭто действие нельзя отменить.",
                "Удалить",
                "Отмена");

            if (!confirm) return;

            try
            {
                var id = SelectedDriver.Id;
                var http = _httpFactory.CreateClient("Api");

                var token = await SecureStorage.Default.GetAsync("auth_token");
                if (!string.IsNullOrEmpty(token))
                {
                    http.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var url = $"{_api.DriversUrl}/{id}";

                // 2. Отправляем запрос DELETE
                var response = await http.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // 3. Успех: Удаляем из локального списка (чтобы он исчез с экрана)
                    // Предполагается, что у вас есть ObservableCollection<Drivers> Drivers
                    if (Drivers.Contains(SelectedDriver))
                    {
                        Drivers.Remove(SelectedDriver);
                    }

                    SelectedDriver = null; // Сбрасываем выбор
                    await Shell.Current.DisplayAlert("Успех", "Водитель успешно удален", "OK");
                }
                else
                {
                    // 4. Ошибка сервера (например, "Нельзя удалить, есть активный маршрут")
                    var errorJson = await response.Content.ReadAsStringAsync();
                    await Shell.Current.DisplayAlert("Ошибка удаления", errorJson, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка сети", ex.Message, "OK");
            }
        }


    }
}
