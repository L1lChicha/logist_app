using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using logist_app.Core.Entities;
using logist_app.Core.Interfaces;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;



namespace logist_app.ViewModels
{
    public partial class RouteCreationViewModel : ObservableObject
    {
        private readonly IClientService _clientService;
        private readonly IRouteService _routeService;
        public RouteCreationViewModel(IClientService clientService, IRouteService routeService)
        {
            _clientService = clientService;
            _routeService = routeService;
        }

        public record AlertMessage(string Title, string Message);

        public ObservableCollection<SelectableClient> SelectedClientsCollection { get; } = new();

        [ObservableProperty]
        public ObservableCollection<SelectableClient> clients = new();

        [ObservableProperty]
        private bool isCreateButtonEnabled = false;

        // Словарь для хранения SelectableClient по Id клиента (сохраняет состояние выделения)
        private Dictionary<int, SelectableClient> _selectableClientsCache = new();

        // Получаем выбранных клиентов
        public List<Client> SelectedClients => _selectableClientsCache.Values
            .Where(c => c.IsSelected)
            .Select(c => c.Client)
            .ToList();

        //[RelayCommand]
        //private void ToggleSelection(SelectableClient client)
        //{
        //    if (client is null) return;
        //    client.IsSelected = !client.IsSelected;
        //    UpdateCreateButtonState();
        //}



        [RelayCommand]
        private void ToggleSelection(SelectableClient client)
        {
            if (client is null) return;

            // Переключаем состояние
            client.IsSelected = !client.IsSelected;

            // 2. СИНХРОНИЗИРУЕМ КОЛЛЕКЦИЮ ДЛЯ ВЕРХНЕГО СПИСКА
            if (client.IsSelected)
            {
                if (!SelectedClientsCollection.Contains(client))
                {
                    SelectedClientsCollection.Add(client);
                }
            }
            else
            {
                if (SelectedClientsCollection.Contains(client))
                {
                    SelectedClientsCollection.Remove(client);
                }
            }

            UpdateCreateButtonState();
        }

        private void UpdateCreateButtonState()
        {
            IsCreateButtonEnabled = _selectableClientsCache.Values.Count(c => c.IsSelected) > 1;
        }

        [RelayCommand]
        private async Task CreateRoute()
        {
            var selectedClients = SelectedClients;
            var response = await _routeService.BuildRoute(selectedClients.Select(c => c.Id).ToList());

            if(response.Response == "OK")
            {
                await Shell.Current.DisplayAlert("Success", "Route saved", "OK");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            
        }

        [RelayCommand]
        public async Task LoadClientsAsync()
        {
            var clientsFromServer = await _clientService.GetClientsAsync();

            allClients = clientsFromServer.ToList();

            // Создаём SelectableClient для каждого клиента и кэшируем
            _selectableClientsCache.Clear();
            foreach (var c in allClients)
            {
                _selectableClientsCache[c.Id] = new SelectableClient(c);
            }

            Clients.Clear();
            foreach (var c in allClients)
                Clients.Add(_selectableClientsCache[c.Id]);
            
            BuildPickerOptionsFromStreets();
            SelectedOptionIndex = 0;
            ApplyFilter(FilterText);
        }





        #region Sorting

        public ObservableCollection<string> PickerOptions { get; } = new();
        private List<string> _streets = new();
        public List<Client> allClients { get; set; } = new();

        [ObservableProperty]
        private int _selectedOptionIndex;

        [ObservableProperty]
        private string _filterText;

        partial void OnSelectedOptionIndexChanged(int value)
        {
            ApplySortOrStreetFilter();
        }
        partial void OnFilterTextChanged(string value)
        {
           
            ApplyFilter(value);

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

            // Обновляем коллекцию, используя кэшированные SelectableClient
            Clients.Clear();
            foreach (var item in query)
            {
                if (_selectableClientsCache.TryGetValue(item.Id, out var selectableClient))
                    Clients.Add(selectableClient);
            }
        }


        public void ApplyFilter(string query)
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
                if (_selectableClientsCache.TryGetValue(c.Id, out var selectableClient))
                    Clients.Add(selectableClient);
            }
        }


        #endregion

    }
}
