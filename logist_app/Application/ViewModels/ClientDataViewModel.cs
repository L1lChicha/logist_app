using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using logist_app.Core.Entities;
using logist_app.Models;
using logist_app.ViewModels;
using logist_app.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace logist_app.ViewModels;

public partial class ClientDataViewModel : ObservableObject
{
    public record AlertMessage(string Title, string Message, string Cancel);
    public ObservableCollection<Client> Clients { get; } = new();
    public List<Client> allClients { get; set; } = new();

    public ObservableCollection<string> PickerOptions { get; } = new();
    private List<string> _streets = new();

    private readonly ApiSettings _api;
    private readonly IHttpClientFactory _httpFactory;

    private string _filterText = string.Empty;
    public string FilterText
    {
        get => _filterText;
        set
        {
            if (_filterText == value) return;
            _filterText = value;
            OnPropertyChanged();
            ApplyFilter(_filterText);
        }
    }

    private int _selectedOptionIndex;
    public int SelectedOptionIndex
    {
        get => _selectedOptionIndex;
        set
        {
            if (_selectedOptionIndex == value) return;
            _selectedOptionIndex = value;
            OnPropertyChanged();
            ApplySortOrStreetFilter(); 
        }
    }

    public ClientDataViewModel(ApiSettings api, IHttpClientFactory httpFactory)
    {
        _api = api;
        _httpFactory = httpFactory;
      
    }


    public async Task LoadDataAsync()
    {
        try
        {
            var http = _httpFactory.CreateClient("Api");
            var routes = await http.GetFromJsonAsync<List<Client>>($"{_api.ClientsEndpoint}");

            allClients = routes ?? new List<Client>();
            Clients.Clear();

            if (routes is not null)
            {
                foreach (var route in allClients)
                    Clients.Add(route);

            }
            else
            {
                WeakReferenceMessenger.Default.Send(new AlertMessage("Ошибка", "Не удалось загрузить маршруты: данные отсутствуют.", "OK"));
            }

            ApplyFilter(FilterText);
            BuildPickerOptionsFromStreets(); 
            SelectedOptionIndex = 0; 
        }
        catch (Exception ex)
        {
            WeakReferenceMessenger.Default.Send(new AlertMessage("Ошибка", $"Не удалось загрузить маршруты: {ex.Message}", "OK"));
        }
    }



    [RelayCommand]
    public async Task RefreshAsync()
    {
        await LoadDataAsync();
    }


    private void BuildPickerOptionsFromStreets()
    {
        // 1) фиксированные первые 2 пункта
        PickerOptions.Clear();
        PickerOptions.Add("Имя (А → Я)");
        PickerOptions.Add("Имя (Я → А)");
        PickerOptions.Add("Дата добавления(Сначала новые)");
        PickerOptions.Add("Дата добавления(Сначала старые)");

        // 2) собрать уникальные улицы из адресов
        var streetSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var c in allClients)
        {
            var street = ExtractStreet(c.Address);
            if (!string.IsNullOrWhiteSpace(street))
                streetSet.Add(street);
        }


        // 3) упорядочить по-русски и добавить в пикер
        var comparer = StringComparer.Create(new CultureInfo("ru-RU"), ignoreCase: true);
        _streets = streetSet.OrderBy(s => s, comparer).ToList();

        foreach (var s in _streets)
            PickerOptions.Add($"Улица {s}");
    }

    private static string? ExtractStreet(string? address)
    {
        if (string.IsNullOrWhiteSpace(address)) return null;

        // Ищем «ул. Пушкина», «улица Кирова», «ул Жукова»
        var m = Regex.Match(address, @"\b(?:ул\.?|улица)\s+([^,]+)", RegexOptions.IgnoreCase);
        if (m.Success)
            return m.Groups[1].Value.Trim();

        // fallback: если адрес вида "Город, Улица Жукова, 12"
        var parts = address.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var streetPart = parts.FirstOrDefault(p => p.Contains("улиц", StringComparison.OrdinalIgnoreCase) ||
                                                   p.Contains("ул", StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(streetPart))
        {
            // уберём префиксы "улица"/"ул."
            streetPart = Regex.Replace(streetPart, @"\b(?:ул\.?|улица)\s*", "", RegexOptions.IgnoreCase).Trim();
            return streetPart;
        }

        // Ничего не нашли — можно попробовать взять вторую часть как улицу
        if (parts.Length >= 2) return parts[1].Trim();

        return null;
    }


    private void ApplySortOrStreetFilter()
    {
        IEnumerable<Client> query = allClients;

        switch (SelectedOptionIndex)
        {
            case 0:
                // Имя (А → Я)
                query = query.OrderBy(c => c.Name, StringComparer.Create(new CultureInfo("ru-RU"), true));
                break;

            case 1:
                // Имя (Я → А)
                query = query.OrderByDescending(c => c.Name, StringComparer.Create(new CultureInfo("ru-RU"), true));
                break;

            case 2:
                // Новые первыми
                query = query.OrderByDescending(c => c.StartDate);
                break;

            case 3:
                // Старые первыми
                query = query.OrderBy(c => c.StartDate);
                break;

            default:
                // индексы 4+ → улицы
                var streetIndex = SelectedOptionIndex - 4;
                if (streetIndex >= 0 && streetIndex < _streets.Count)
                {
                    var street = _streets[streetIndex];
                    query = query.Where(c => ExtractStreet(c.Address)
                                ?.Equals(street, StringComparison.OrdinalIgnoreCase) == true)
                                 .OrderBy(c => c.Address, StringComparer.Create(new CultureInfo("ru-RU"), true));
                }
                break;
        }

        // Обновляем коллекцию
        Clients.Clear();
        foreach (var item in query)
            Clients.Add(item);
    }


    public void ApplyFilter(string? query)
    {
        var q = (query ?? string.Empty).Trim().ToLowerInvariant();

        Clients.Clear();
        foreach (var c in allClients.Where(c =>
                 string.IsNullOrEmpty(q) ||
                 (c.Name?.ToLowerInvariant().Contains(q) ?? false) ||
                 (c.Address?.ToLowerInvariant().Contains(q) ?? false) ||
                 (c.City?.ToLowerInvariant().Contains(q) ?? false) ||
                 (c.Email?.ToLowerInvariant().Contains(q) ?? false) ||
                 (c.Phone?.ToLowerInvariant().Contains(q) ?? false)))
        {
            Clients.Add(c);
        }
    }




    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));









    [ObservableProperty]
    private Client selectedClient;

    partial void OnSelectedClientChanged(Client value)
    {
        if (value != null)
            _ = OpenClientAsync(value);
    }

    private EditClientViewModel editVM;
      
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





}
