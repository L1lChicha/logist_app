using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using logist_app.Core.Entities; // Убедитесь, что RecurrenceSettings здесь
using logist_app.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace logist_app.ViewModels;

public partial class EditClientViewModel : ObservableObject
{
    [ObservableProperty]
    private Client client;

    // Свойство для отображения краткой информации о расписании
    [ObservableProperty]
    private string recurrenceSummary;

    private readonly ApiSettings _api;
    private readonly IHttpClientFactory _httpFactory;

    public EditClientViewModel(ApiSettings apiSettings, IHttpClientFactory httpFactory)
    {
        _api = apiSettings;
        _httpFactory = httpFactory;
    }

    public void Initialize(Client client)
    {
        Client = client;

        // Если у клиента нет расписания, создаем дефолтное
        if (Client.Schedule == null)
        {
            Client.Schedule = new RecurrenceSettings();
        }

        UpdateRecurrenceSummary();
    }

    // Команда открытия окна настройки расписания
    [RelayCommand]
    private async Task ConfigureRecurrence()
    {
        //if (Client.Schedule == null)
        //{
        //    Client.Schedule = new RecurrenceSettings();
        //}

        // Создаем страницу, передаем текущие настройки и Action callback
        var recurrencePage = new RecurrenceModalPage(Client.Schedule, (newSettings) =>
        {
            // Callback: когда пользователь сохранит настройки в модальном окне
            Client.Schedule = newSettings;
            UpdateRecurrenceSummary();
        });

        // Открываем модально
        await Shell.Current.Navigation.PushModalAsync(recurrencePage);
    }

    // Метод обновления текста сводки
    private void UpdateRecurrenceSummary()
    {
        if (Client.Schedule == null)
        {
            RecurrenceSummary = "No schedule set";
            return;
        }

        string summary = $"{Client.Schedule.Type}, Interval: {Client.Schedule.Interval}";

        if (Client.Schedule.Type == "Weekly" && Client.Schedule.DaysOfWeek.Any())
        {
            summary += $"\nDays: {string.Join(", ", Client.Schedule.DaysOfWeek)}";
        }

        if (Client.Schedule.WeeksOfMonth.Any())
        {
            summary += $"\nWeeks: {string.Join(", ", Client.Schedule.WeeksOfMonth)}";
        }

        if (Client.Schedule.Type == "Monthly" && Client.Schedule.DaysOfMonth.Any())
        {
            summary += $"\nDays of month: {string.Join(", ", Client.Schedule.DaysOfMonth)}";
        }

        RecurrenceSummary = summary;
    }

    [RelayCommand]
    private async Task SaveClientAsync()
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

        var response = await http.PutAsJsonAsync($"{_api.ClientsEndpoint}/{client.Id}", Client);

        if (response.IsSuccessStatusCode)
        {
            await Shell.Current.DisplayAlert("Success", "Client updated.", "OK");
            await Shell.Current.Navigation.PopAsync(); // Возвращаемся назад после сохранения
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            await Shell.Current.DisplayAlert("Error", $"Failed to update client.\n{error}", "OK");
        }
    }

    [RelayCommand]
    private async Task DeleteClient()
    {
        bool confirm = await Shell.Current.DisplayAlert("Confirm", $"Delete {Client.Name}?", "Yes", "No");
        if (!confirm) return;

        var http = _httpFactory.CreateClient("Api");

        // --- ДОБАВЛЯЕМ ТОКЕН ---
        var token = await SecureStorage.Default.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
        {
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
        // -----------------------

        var response = await http.DeleteAsync($"{_api.ClientsEndpoint}/{Client.Id}");

        if (response.IsSuccessStatusCode)
        {
            await Shell.Current.DisplayAlert("Deleted", "Client deleted.", "OK");
            await Shell.Current.Navigation.PopAsync();
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            await Shell.Current.DisplayAlert("Error", $"Failed to delete client.\n{error}", "OK");
        }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.Navigation.PopAsync();
    }
}