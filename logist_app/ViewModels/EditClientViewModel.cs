using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Net.Http.Json;
using logist_app.Models;

namespace logist_app.ViewModels;

public class EditClientViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private ClientViewModel _client;
    public ClientViewModel Client
    {
        get => _client;
        set { _client = value; OnPropertyChanged(); }
    }

    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand CancelCommand { get; }

    private readonly INavigation _navigation;
    private readonly Func<Task> _refreshCallback;

    // ✅ Новые поля для DI
    private readonly ApiSettings _apiSettings;
    private readonly IHttpClientFactory _httpClientFactory;

    // ✅ Обновлённый конструктор
    public EditClientViewModel(
        ClientViewModel client,
        INavigation navigation,
        Func<Task> refreshCallback,
        ApiSettings apiSettings,
        IHttpClientFactory httpClientFactory)
    {
        Client = client;
        _navigation = navigation;
        _refreshCallback = refreshCallback;
        _apiSettings = apiSettings;
        _httpClientFactory = httpClientFactory;

        SaveCommand = new Command(async () => await SaveClientAsync());
        DeleteCommand = new Command(async () => await DeleteClientAsync());
        CancelCommand = new Command(async () => await CancelAsync());
    }

    private async Task SaveClientAsync()
    {
        var http = _httpClientFactory.CreateClient("Api");
        var response = await http.PutAsJsonAsync($"{_apiSettings.ClientsEndpoint}/{Client.Id}", Client);

        if (response.IsSuccessStatusCode)
        {
            await Application.Current.MainPage.DisplayAlert("Success", "Client updated.", "OK");
            await _refreshCallback();
            await _navigation.PopAsync();
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            await Application.Current.MainPage.DisplayAlert("Error", $"Failed to update client.\n{error}", "OK");
        }
    }

    private async Task DeleteClientAsync()
    {
        bool confirm = await Application.Current.MainPage.DisplayAlert("Confirm", $"Delete {Client.Name}?", "Yes", "No");
        if (!confirm) return;

        var http = _httpClientFactory.CreateClient("Api");
        var response = await http.DeleteAsync($"{_apiSettings.ClientsEndpoint}/{Client.Id}");

        if (response.IsSuccessStatusCode)
        {
            await Application.Current.MainPage.DisplayAlert("Deleted", "Client deleted.", "OK");
            await _refreshCallback();
            await _navigation.PopAsync();
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            await Application.Current.MainPage.DisplayAlert("Error", $"Failed to delete client.\n{error}", "OK");
        }
    }

    private async Task CancelAsync()
    {
        await _refreshCallback();
        await _navigation.PopAsync();
    }

    private void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
