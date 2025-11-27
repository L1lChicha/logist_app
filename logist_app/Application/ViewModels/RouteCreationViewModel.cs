using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using logist_app.Core.Entities;
using logist_app.Core.Interfaces;
using System.Collections.ObjectModel;



namespace logist_app.ViewModels
{
    public partial class RouteCreationViewModel : ObservableObject
    {
        private readonly IClientService _clientService;
        public RouteCreationViewModel(IClientService clientService)
        {
            _clientService = clientService;
        }

 
        [ObservableProperty]
        public ObservableCollection<Client> clients = new();

        [ObservableProperty]
        private List<Client> selectedClients= new();

        [ObservableProperty]
        private bool isCreateButtonEnabled = false;




        [RelayCommand]
        private void SelectionChanged(object? selectedItems)
        {
            var list = selectedItems as IList<object>;
            if (list is null) return;
            SelectedClients = list.Cast<Client>().ToList();
            IsCreateButtonEnabled = SelectedClients.Count > 1;
        }



        [RelayCommand]
        public async Task LoadClientsAsync()
        {
            var clients = await _clientService.GetClientsAsync();
            Clients.Clear();
            foreach (var c in clients)
                Clients.Add(c);
        }
    }
}
