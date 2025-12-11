using logist_app.Core.Entities;
using logist_app.Infrastructure.Service;
using Microsoft.Maui.Storage; // Не забудьте

namespace logist_app
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }
        private readonly SignalRService _signalRService;

        public App(IServiceProvider serviceProvider, SignalRService signalRService)
        {
            InitializeComponent();

            Services = serviceProvider;
            _signalRService = signalRService;

            // ПОДПИСКА - ЭТО ПРАВИЛЬНО. Оставляем здесь.
            _signalRService.OnNoteReceived += HandleNotification;

            // УДАЛИТЕ ЭТУ СТРОКУ! 👇
            // Task.Run(async () => await _signalRService.ConnectAsync()); 
            // Мы сделаем это позже, только если есть токен.
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // 1. Создаем Shell и сохраняем в переменную
            var shell = new AppShell();
            var window = new Window(shell);

            // 2. Умная проверка при запуске
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                // Проверяем токен
                var token = await SecureStorage.Default.GetAsync("auth_token");

                if (!string.IsNullOrEmpty(token))
                {
                    // Если токен есть:

                    // А. Переходим на главную (убедитесь, что в AppShell.xaml есть Route="ActionPageView")
                    await shell.GoToAsync("//ActionPageView");

                    // Б. Теперь безопасно подключаемся
                    await _signalRService.ConnectAsync();
                }
                else
                {
                    // Если токена нет — пользователь останется на экране Входа (первая вкладка в Shell)
                    // и SignalR НЕ будет спамить ошибками 401.
                }
            });

            return window;
        }

        // ... Ваши методы HandleNotification (оставьте как есть) ...
        private void HandleNotification(ClientNoteNotification note)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "🔔 Новая проблема!",
                        $"Клиент: {note.ClientId}\nЗаметка: {note.NotesAboutProblems}",
                        "ОК"
                    );
                }
            });
        }
    }
}