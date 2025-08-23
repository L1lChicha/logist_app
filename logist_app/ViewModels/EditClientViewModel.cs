using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;

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


    private const string ApiUrl = "https://localhost:32769/api/Clients";
    private readonly INavigation _navigation;
    private readonly Func<Task> _refreshCallback;

    public EditClientViewModel(ClientViewModel client, INavigation navigation, Func<Task> refreshCallback)
    {
        Client = client;
        _navigation = navigation;
        _refreshCallback = refreshCallback;

        SaveCommand = new Command(async () => await SaveClientAsync());
        DeleteCommand = new Command(async () => await DeleteClientAsync());
        CancelCommand = new Command(async () => await CancelAsync());

    }

    private async Task SaveClientAsync()
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.PutAsJsonAsync($"{ApiUrl}/{Client.id}", Client);

        if (response.IsSuccessStatusCode)
        {
            await Application.Current.MainPage.DisplayAlert("Success", "Client updated.", "OK");
            await _refreshCallback();
            await _navigation.PopAsync();
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Failed to update client.", "OK");
        }
    }

    private async Task CancelAsync()
    {
        await _refreshCallback(); // если нужно обновление при отмене
        await _navigation.PopAsync();
    }



    private async Task DeleteClientAsync()
    {
        bool confirm = await Application.Current.MainPage.DisplayAlert("Confirm", $"Delete {Client.name}?", "Yes", "No");
        if (!confirm) return;

        using var httpClient = new HttpClient();
        var response = await httpClient.DeleteAsync($"{ApiUrl}/{Client.id}");

        if (response.IsSuccessStatusCode)
        {
            await Application.Current.MainPage.DisplayAlert("Deleted", "Client deleted.", "OK");
            await _refreshCallback();
            await _navigation.PopAsync();
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete client.", "OK");
        }
    }

    private void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
