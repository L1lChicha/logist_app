using logist_app.Core.Entities;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Storage; // Убедитесь, что этот using есть для SecureStorage

namespace logist_app.Infrastructure.Service
{
    public class SignalRService
    {
        private HubConnection _hubConnection;

        // Событие для передачи данных во ViewModel/UI
        public event Action<ClientNoteNotification> OnNoteReceived;

        // Поле для URL (лучше вынести в константы или настройки)
        private const string HUB_URL = "https://esme-aspiratory-september.ngrok-free.dev/hubs/notifications";

        public SignalRService()
        {
            // Инициализация перенесена в метод Init, или можно оставить здесь, 
            // но НЕ вызывайте .Result для токена!
            InitializeConnection();
        }

        private void InitializeConnection()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(HUB_URL, options =>
                {
                    // ИСПРАВЛЕНИЕ 1: Правильная передача токена
                    // AccessTokenProvider ожидает функцию, возвращающую Task<string>.
                    // Мы просто передаем ссылку на наш асинхронный метод.
                    options.AccessTokenProvider = GetJwtTokenAsync;

                    // Заголовок для обхода защиты ngrok
                    options.Headers.Add("ngrok-skip-browser-warning", "true");
                })
                .WithAutomaticReconnect() // Авто-переподключение при обрыве
                .Build();

            // Подписка на сообщения от сервера
            _hubConnection.On<ClientNoteNotification>("ClientNoteUpdated", (data) =>
            {
                // Прокидываем данные подписчикам
                OnNoteReceived?.Invoke(data);
            });

            _hubConnection.Closed += (error) =>
            {
                Console.WriteLine($"SignalR Connection Closed: {error?.Message}");
                return Task.CompletedTask;
            };
        }

        // Хелпер для получения токена (вызывается самим SignalR внутри)
        private async Task<string> GetJwtTokenAsync()
        {
            // Убедитесь, что ключ совпадает с тем, что вы сохраняете при Логине!
            // В вашем коде было то "auth_token", то "access_token". Я оставил один.
            //return await SecureStorage.Default.GetAsync("auth_token");
            return await SecureStorage.Default.GetAsync("auth_token");
        }

        public async Task ConnectAsync()
        {
            if (_hubConnection == null) InitializeConnection();

            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    Console.WriteLine("Попытка подключения к SignalR...");
                    await _hubConnection.StartAsync();

                    // Если у вас есть группы на сервере
                    await _hubConnection.InvokeAsync("JoinLogisticsGroup");

                    Console.WriteLine("SignalR: Успешно подключено!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SignalR Error: {ex.Message}");
                }
            }
        }

        public async Task DisconnectAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
            }
        }
    }
}