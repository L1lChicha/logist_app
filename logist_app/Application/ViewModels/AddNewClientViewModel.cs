using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using logist_app.Core.Entities;
using logist_app.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Xml.Linq;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace logist_app.ViewModels;

public partial class AddNewClientViewModel : ObservableObject
{
    private readonly ApiSettings _apiSettings;
    private readonly IHttpClientFactory _httpClientFactory;

    // Form Properties
    [ObservableProperty] private string name;
    [ObservableProperty] private string address;
    [ObservableProperty] private string city;
    [ObservableProperty] private string postalCode;
    [ObservableProperty] private string phone;
    [ObservableProperty] private string email;
    [ObservableProperty] private string containerCount;
    [ObservableProperty] private string volume;
    [ObservableProperty] private string loadingType;
    [ObservableProperty] private DateTime startDate = DateTime.Today;

    // Recurrence
    [ObservableProperty] private RecurrenceSettings recurrenceSettings = new();
    [ObservableProperty] private string recurrenceSummary = "Weekly (Every 1 week)";

    // Location Data
    [ObservableProperty] private double lat;
    [ObservableProperty] private double lon;

    public AddNewClientViewModel(ApiSettings apiSettings, IHttpClientFactory httpClientFactory)
    {
        _apiSettings = apiSettings;
        _httpClientFactory = httpClientFactory;
    }

    [RelayCommand]
    private async Task Submit()
    {
        // Validation
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Address) ||
            string.IsNullOrWhiteSpace(City) || string.IsNullOrWhiteSpace(PostalCode) ||
            string.IsNullOrWhiteSpace(Phone) || string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(ContainerCount) || string.IsNullOrWhiteSpace(LoadingType) ||
            string.IsNullOrWhiteSpace(Volume))
        {
            await Shell.Current.DisplayAlert("Error", "Please fill in all form fields.", "OK");
            return;
        }

        if (!int.TryParse(ContainerCount, out int parsedContainerCount))
        {
            await Shell.Current.DisplayAlert("Error", "The number of containers must be a number.", "OK");
            return;
        }

        if (!double.TryParse(Volume, out double parsedVolume))
        {
            await Shell.Current.DisplayAlert("Error", "Volume must be a number.", "OK");
            return;
        }

        var newClient = new Client
        {
            Name = Name,
            Address = Address,
            City = City,
            PostalCode = PostalCode,
            Phone = Phone,
            Email = Email,
            ContainerCount = parsedContainerCount,
            StartDate = StartDate.ToUniversalTime(),
            Coordinates = $"{Lat}, {Lon}",
            LoadingType = LoadingType,
            Volume = parsedVolume,
            Lat = Lat,
            Lon = Lon,
            Schedule = RecurrenceSettings
        };

        if (await AddClientApiCall(newClient))
        {
            await Shell.Current.DisplayAlert("Success", "Client added successfully.", "OK");
            await Shell.Current.Navigation.PopAsync();
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "Failed to add client.", "OK");
        }
    }

    private async Task<bool> AddClientApiCall(Client client)
    {
        try
        {
            var clientHttp = _httpClientFactory.CreateClient("Api");

            // --- ДОБАВЛЯЕМ ТОКЕН ---
            var token = await SecureStorage.Default.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                clientHttp.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            // -----------------------

            var response = await clientHttp.PostAsJsonAsync(_apiSettings.ClientsEndpoint, client);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to add client {ex.Message}", "OK");
            return false;
        }
    }

    // Updates the summary label text
    public void UpdateRecurrenceSummary()
    {
        string summary = $"{RecurrenceSettings.Type}, Interval: {RecurrenceSettings.Interval}";

        if (RecurrenceSettings.Type == "Weekly" && RecurrenceSettings.DaysOfWeek.Any())
        {
            summary += $"\nDays: {string.Join(", ", RecurrenceSettings.DaysOfWeek)}";
        }

        if (RecurrenceSettings.WeeksOfMonth.Any())
        {
            summary += $"\nWeeks: {string.Join(", ", RecurrenceSettings.WeeksOfMonth)}";
        }
        RecurrenceSummary = summary;
    }

    // Helper for Geocoding (Logic moved from View)
    public async Task<(double lat, double lon, string displayName)?> GeocodeAddressAsync(string query)
    {
        var url = $"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(query)}&accept-language=ru";
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "MyTestApp");

        try
        {
            var response = await httpClient.GetFromJsonAsync<List<NominatimResult>>(url);
            if (response == null || response.Count == 0) return null;

            var result = response.FirstOrDefault(r =>
               r.display_name.Contains("Брест", StringComparison.OrdinalIgnoreCase) ||
               r.display_name.Contains("Brest", StringComparison.OrdinalIgnoreCase));

            if (result == null) return null;

            var lat = double.Parse(result.lat, CultureInfo.InvariantCulture);
            var lon = double.Parse(result.lon, CultureInfo.InvariantCulture);

            return (lat, lon, result.display_name);
        }
        catch
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
}