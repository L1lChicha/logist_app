
using Microsoft.Maui.Controls;
using Npgsql;
using System;
using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace logist_app;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        // MapWebView.Source = "map.html";
    }


    private (double lat, double lon)? point;
    private string addressName;

    private double lat;
    private double lon;
    //получение данных метки на карте
    private async void GetLocationData(object sender, EventArgs e)
    {
        try
        {
            // Вызов JavaScript метода и получение JSON-результата
            var json = await MapWebView.EvaluateJavaScriptAsync("getSelectedLocation()");
            json = json.Trim('"').Replace("\\", "");

            // Парсинг JSON в объект
            var data = JsonSerializer.Deserialize<MapData>(json);

            if (data != null)
            {
                lat = data.lat;
                lon = data.lon;

                //установка данных в поля
                string rawInputAddress = data.address;
                // string inputAddress = rawInputAddress;
                string[] addressDetails = rawInputAddress.Split(',');

                if (addressDetails[4] == " Брест")
                {
                    AddressEntry.Text = addressDetails[1].Trim() + ", " + addressDetails[0];
                    CityEntry.Text = addressDetails[4].Trim();
                    PostalCodeEntry.Text = addressDetails[6].Trim();
                }
                // Используйте данные
                //await DisplayAlert("Данные с карты", $"Адрес: {data.address}\nШирота: {data.lat}\nДолгота: {data.lon}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "", "OK");
        }
    }
    public class MapData
    {
        public double lat { get; set; } // Для числовых значений широты
        public double lon { get; set; } // Для числовых значений долготы
        public string address { get; set; } // Для строки с адресом
    }




    //нахождение адреса по строке 
    private async void FindButton(object sender, EventArgs e)
    {
        string address = AddressEntry.Text;
        if (address != null)
        {
            var result = await GeocodeAddress(address);
            if (result is (double lat, double lon, string displayName))
            {
                point = (lat, lon);
                addressName = displayName;
                string jsCode = $"setMarker({lat.ToString(CultureInfo.InvariantCulture)}, {lon.ToString(CultureInfo.InvariantCulture)}, '{displayName.Replace("'", "\\'")}');";
                await MapWebView.EvaluateJavaScriptAsync(jsCode);
            }
        }
        else
        {
            await DisplayAlert("Error", "The address field must be filled", "OK");
        }
        
    }

    public async Task<(double lat, double lon, string displayName)?> GeocodeAddress(string query)
    {
        var url = $"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(query)}&accept-language=ru";
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "MyTestApp");

        var response = await httpClient.GetFromJsonAsync<List<NominatimResult>>(url);

        if (response == null || response.Count == 0)
        {
            return null;
        }

        var brestResult = response.FirstOrDefault(r =>
            r.display_name.Contains("Брест", StringComparison.OrdinalIgnoreCase) ||
            r.display_name.Contains("Brest", StringComparison.OrdinalIgnoreCase)
        );

        if (brestResult == null)
        {
            return null;
        }

        try
        {
            var lat = double.Parse(brestResult.lat, CultureInfo.InvariantCulture);
            var lon = double.Parse(brestResult.lon, CultureInfo.InvariantCulture);
            if (lat < -90 || lat > 90 || lon < -180 || lon > 180)
            {
                return null;
            }
            return (lat, lon, brestResult.display_name);
        }
        catch (FormatException ex)
        {

            return null;
        }
    }

    public class NominatimResult
    {
        public string lat { get; set; }
        public string lon { get; set; }
        public string display_name { get; set; }
    }


    //сохранение данных в базу данных

    private async void OnSubmitClicked(object sender, EventArgs e)
    {

        // Собираем данные из формы
        string name = NameEntry.Text;
        string address = AddressEntry.Text;
        string city = CityEntry.Text;
        string postalCode = PostalCodeEntry.Text;
        string phone = PhoneEntry.Text;
        string email = EmailEntry.Text;
        string recurrence = RecurrencePicker.SelectedItem?.ToString();
        string containerCountText = ContainerCountEntry.Text;
        DateTime startDate = StartDatePicker.Date;
        string coordinates = lat + ", " + lon;
       

        // Валидация данных
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(address) ||
            string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(postalCode) ||
            string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(recurrence) || string.IsNullOrWhiteSpace(containerCountText))
        {
            await DisplayAlert("Error", "Please fill in all form fields.", "OK");
            return;
        }

        if (!int.TryParse(containerCountText, out int containerCount))
        {
            await DisplayAlert("Error", "The number of containers must be a number.", "OK");
            return;
        }


        var newClient = new ClientViewModel();
        newClient.name = name;
        newClient.address = address;
        newClient.city = city;
        newClient.postal_code = postalCode;
        newClient.phone = phone;
        newClient.email = email;
        newClient.recurrence = recurrence;
        newClient.container_count = containerCount;
        newClient.start_date = startDate.ToUniversalTime();
        newClient.coordinates = coordinates;
        newClient.lat = lat;
        newClient.lon = lon;

        var success = await AddNewClientAsync(newClient);
        if (success)
        {
            await DisplayAlert("Success", "Client added successfully.", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Failed to add client.", "OK");
        }
    }

    private async Task<bool> AddNewClientAsync(ClientViewModel newClient)
    {
        var ApiUrl = "https://localhost:32771/api/Clients";

        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync(ApiUrl, newClient);
            var json = JsonSerializer.Serialize(newClient);



            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding client: {ex.Message}");
            return false;
        }
    }


    //просмотр данных из базы данных

    private async void OnViewDataClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DataViewPage());
    }
}