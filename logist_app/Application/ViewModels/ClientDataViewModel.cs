using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using logist_app.Core.Entities;
using logist_app.Core.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq; // Не забудьте добавить Linq

namespace logist_app.ViewModels;

public partial class ClientDataViewModel : ObservableObject
{
    public record AlertMessage(string Title, string Message, string Cancel);

    private readonly IClientService clientService;

    // Храним ВСЕХ загруженных клиентов здесь (резервная копия для фильтрации)
    private List<Client> _allClients = new();

    public ClientDataViewModel(IClientService _clientService)
    {
        clientService = _clientService;

        // Инициализация вариантов сортировки
        SortOptions = new ObservableCollection<string>
        {
            "Default (ID)",
            "Name (A-Z)",
            "City",
            "Num. of containers"
        };
        SelectedSortOption = SortOptions[0];
    }

    // Коллекция для Picker'а
    [ObservableProperty]
    private ObservableCollection<string> sortOptions;

    // Выбранный вариант сортировки
    [ObservableProperty]
    private string selectedSortOption;

    // Текст поиска
    [ObservableProperty]
    private string searchText;

    // Отображаемая коллекция
    [ObservableProperty]
    private ObservableCollection<Client> clients = new();

    [ObservableProperty]
    private Client selectedClient;

    // Хук: срабатывает автоматически при изменении SearchText
    partial void OnSearchTextChanged(string value)
    {
        FilterAndSort();
    }

    // Хук: срабатывает автоматически при изменении SelectedSortOption
    partial void OnSelectedSortOptionChanged(string value)
    {
        FilterAndSort();
    }

    partial void OnSelectedClientChanged(Client value)
    {
        if (value != null)
            _ = OpenClientAsync(value);
    }

    // Основной метод фильтрации и сортировки
    private void FilterAndSort()
    {
        if (_allClients is null || !_allClients.Any()) return;

        IEnumerable<Client> filtered = _allClients;

        // 1. Фильтрация (Поиск)
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var lowerTerm = SearchText.ToLower();
            filtered = filtered.Where(c =>
                (c.Name != null && c.Name.ToLower().Contains(lowerTerm)) ||
                (c.Address != null && c.Address.ToLower().Contains(lowerTerm)) ||
                (c.City != null && c.City.ToLower().Contains(lowerTerm)) ||
                (c.Phone != null && c.Phone.Contains(lowerTerm))
            );
        }

        // 2. Сортировка
        filtered = SelectedSortOption switch
        {
            "Name (A-Z)" => filtered.OrderBy(c => c.Name),
            "City" => filtered.OrderBy(c => c.City),
            "Num. of containers" => filtered.OrderByDescending(c => c.ContainerCount),
            _ => filtered.OrderBy(c => c.Id) // По умолчанию
        };

        // 3. Обновление UI
        Clients.Clear();
        foreach (var c in filtered)
        {
            Clients.Add(c);
        }
    }

    [RelayCommand]
    private async Task OpenClientAsync(Client client)
    {
        if (client == null)
        {
            await Shell.Current.DisplayAlert("Error", "Client not found.", "OK");
            return;
        }

        // Получаем сервис для страницы редактирования (убедитесь, что EditClientPage зарегистрирована в MauiProgram.cs)
        // Лучше использовать Dependency Injection в конструкторе, если возможно, но оставим ваш подход:
        var page = App.Services.GetService<EditClientPage>(); // Или new EditClientPage(vm)

        // Предполагаем, что у EditClientPage есть метод Initialize или BindingContext
       
        page.Initialize(client); 

        await Shell.Current.Navigation.PushAsync(page);
        SelectedClient = null;
    }

    [RelayCommand]
    public async Task LoadClientsAsync()
    {
        // Проверяем _allClients, так как Clients может быть пустым из-за фильтра
        if (_allClients.Count != 0)
            return;

        try
        {
            var clientsFromServer = await clientService.GetClientsAsync();

            _allClients.Clear();
            _allClients.AddRange(clientsFromServer);

            FilterAndSort(); // Заполняет Clients с учетом текущих фильтров
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Loading error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    public async Task Refresh()
    {
        _allClients.Clear();
        Clients.Clear();
        await LoadClientsAsync();
    }
}