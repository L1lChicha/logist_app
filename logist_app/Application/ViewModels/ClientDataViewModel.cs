using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using logist_app.Core.Entities;
using logist_app.Core.Interfaces;
using System.Collections.ObjectModel;


namespace logist_app.ViewModels;

public partial class ClientDataViewModel : ObservableObject
{
    public record AlertMessage(string Title, string Message, string Cancel);

    

    private readonly IClientService clientService;



    public ClientDataViewModel(IClientService _clientService)
    {
        clientService = _clientService;
    }

    [ObservableProperty]
    private ObservableCollection<Client> clients = new();



    [ObservableProperty]
    private Client selectedClient;

    partial void OnSelectedClientChanged(Client value)
    {
        if (value != null)
            _ = OpenClientAsync(value);
    }
   
    [RelayCommand]
    private async Task OpenClientAsync(Client client)
    {
        if (client == null)
        {
            WeakReferenceMessenger.Default.Send(new AlertMessage("Ошибка", "Маршрут не содержит данных геометрии.", "OK"));
            return;
        }

        Client clientToShow = client;
        var page = App.Services.GetService<EditClientPage>();
        page.Initialize(clientToShow);

        await Shell.Current.Navigation.PushAsync(page);
        SelectedClient = null;
    }

    [RelayCommand]
    public async Task LoadClientsAsync()
    {
        var clientsFromServer = await clientService.GetClientsAsync();

        Clients.Clear();
        foreach (var c in clientsFromServer)
            Clients.Add(c);

    }

    [RelayCommand]
    public async Task Refresh()
    {
        await LoadClientsAsync();
    }


}
