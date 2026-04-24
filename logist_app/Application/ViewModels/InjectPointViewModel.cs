using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using logist_app.Core.Entities;
using logist_app.Core.Interfaces;
using logist_app.Infrastructure.Service;
using logist_app.Infrastructure.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace logist_app.ViewModels
{
    public partial class InjectPointViewModel : ObservableObject
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IClientService _clientService;

        // --- Состояние переключателя ---
        [ObservableProperty]
        private bool isExistingMode = true; // По умолчанию "Из базы"

        [ObservableProperty]
        private bool isNewMode = false;

        // --- Данные для "Существующего" ---
        public ObservableCollection<Client> Clients { get; } = new();

        [ObservableProperty]
        private Client selectedClient;

        // --- Данные для "Нового" ---
        [ObservableProperty] private string newName;
        [ObservableProperty] private string newAddress;
        [ObservableProperty] private double? newLat;
        [ObservableProperty] private double? newLon;
        [ObservableProperty] private int containerCount = 1;

        public InjectPointViewModel(IHttpClientFactory httpFactory, IClientService clientService)
        {
            _clientService = clientService;
            _httpFactory = httpFactory;
            LoadClientsAsync();
        }

        // Переключатель режимов
        [RelayCommand]
        void SwitchMode(string mode)
        {
            if (mode == "Existing")
            {
                IsExistingMode = true;
                IsNewMode = false;
            }
            else
            {
                IsExistingMode = false;
                IsNewMode = true;
            }
        }

        // Загрузка списка клиентов для Picker
        async Task LoadClientsAsync()
        {
            try
            {
                var http = _httpFactory.CreateClient("Api");
                // Предполагаем, что есть эндпоинт для получения списка
                var list = await _clientService.GetClientsAsync(); // Укажите ваш путь
                if (list != null)
                {
                    Clients.Clear();
                    foreach (var c in list) Clients.Add(c);
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибки
                Console.WriteLine(ex.Message);
            }
        }

        [RelayCommand]
        async Task InjectPointAsync()
        {
            var dto = new InjectPointDto();

            if (IsExistingMode)
            {
                if (SelectedClient == null)
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Выберите клиента из списка", "OK");
                    return;
                }
                dto.ExistingClientId = SelectedClient.Id;
            }
            else
            {
                if (!NewLat.HasValue || !NewLon.HasValue)
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Введите координаты", "OK");
                    return;
                }
                dto.NewClientName = string.IsNullOrWhiteSpace(NewName) ? "Срочная точка" : NewName;
                dto.NewClientAddress = NewAddress;
                dto.NewClientLat = NewLat;
                dto.NewClientLon = NewLon;
                dto.ContainerCount = ContainerCount;
            }

            try
            {
                var http = _httpFactory.CreateClient("Api");

                var response = await http.PostAsJsonAsync("api/Route/inject-smart", dto);

                if (response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert("Успех", "Точка успешно добавлена в маршрут!", "OK");
                    await Shell.Current.Navigation.PopAsync();  
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    await Shell.Current.DisplayAlert("Ошибка", error, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }
    }
}