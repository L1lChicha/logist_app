using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using logist_app.Models;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace logist_app.ViewModels;

public partial class EditClientViewModel : ObservableObject
{
    [ObservableProperty]
    private Client client;

    private readonly ApiSettings _api;
    private readonly IHttpClientFactory _httpFactory;
    public void Initialize(Client client)
    {
        Client = client;
    }

    public EditClientViewModel(ApiSettings apiSettings, IHttpClientFactory httpFactory)
    {
        _api = apiSettings;
        _httpFactory = httpFactory;

    }


    [RelayCommand]
    private async Task SaveClientAsync()
    {
        var http = _httpFactory.CreateClient("Api");
        var response = await http.PutAsJsonAsync($"{_api.ClientsEndpoint}/{client.Id}", client);

        if (response.IsSuccessStatusCode)
        {
            await Application.Current.MainPage.DisplayAlert("Success", "Client updated.", "OK");

        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            await Application.Current.MainPage.DisplayAlert("Error", $"Failed to update client.\n{error}", "OK");
        }
    }

    [RelayCommand]
    private async Task DeleteClient()
     {
        bool confirm = await Application.Current.MainPage.DisplayAlert("Confirm", $"Delete {Client.Name}?", "Yes", "No");
        if (!confirm) return;

        var http = _httpFactory.CreateClient("Api");
        var response = await http.DeleteAsync($"{_api.ClientsEndpoint}/{Client.Id}");

        if (response.IsSuccessStatusCode)
        {
            await Application.Current.MainPage.DisplayAlert("Deleted", "Client deleted.", "OK");
        
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            await Application.Current.MainPage.DisplayAlert("Error", $"Failed to delete client.\n{error}", "OK");
        }
    }
    [RelayCommand]
    private async Task Cancel()
    {

        await Application.Current.MainPage.Navigation.PopAsync();
    }

}
