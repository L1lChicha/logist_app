using logist_app.Core.Entities;
using logist_app.Infrastructure.Service;
using logist_app.Views;
using Microsoft.Toolkit.Uwp.Notifications;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using System.IdentityModel.Tokens.Jwt;
using Windows.UI.Notifications;

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

            _signalRService.OnNoteReceived += HandleNotification;
            _signalRService.OnNoteReceived += HandleGlobalNotification;

            ToastNotificationManagerCompat.OnActivated += OnWindowsToastActivated;

        }
        private void OnNotificationClicked(NotificationActionEventArgs e)
        {
            if (e.IsDismissed) return; // Если пользователь просто смахнул уведомление, ничего не делаем

            if (e.IsTapped) // Если пользователь нажал на уведомление
            {
                // UI операции обязательно выполнять в главном потоке
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    // Переходим на страницу уведомлений
                    // Используем Shell.Current.GoToAsync
                    await Shell.Current.GoToAsync(nameof(NotificationsView));
                });
            
            }
        }



        private void OnWindowsToastActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            // 1. Получаем аргументы
            var args = ToastArguments.Parse(e.Argument);

            // 2. ОБЯЗАТЕЛЬНО: Переходим в главный поток, чтобы работать с UI
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                // 3. МАГИЯ: Разворачиваем окно приложения
                var window = Current.Windows.FirstOrDefault();
                if (window != null)
                {
                    // Получаем нативное окно Windows
                    var nativeWindow = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;

                    // Команда "показать окно" и "вынести на передний план"
                    nativeWindow?.Activate();

                    // Если окно было свернуто в панель задач, иногда нужно восстановить его.
                    // В большинстве случаев Activate() хватает, но если не сработает,
                    // напишите, добавлю код для Win32 API.
                }

                // 4. Логика навигации
                if (args.TryGetValue("action", out string action) && action == "openClient")
                {
                    if (args.TryGetValue("clientId", out string clientId))
                    {
                        // Переход на страницу
                        await Shell.Current.GoToAsync($"{nameof(NotificationsView)}?clientId={clientId}");
                    }
                }
                else
                {
                    // Если аргументов нет, просто открываем уведомления
                    await Shell.Current.GoToAsync(nameof(NotificationsView));
                }
            });
        }

        private void NavigateToClient(string clientId)
        {
            // Важно: навигацию делаем в главном потоке
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                // Пример перехода на страницу уведомлений или клиента
                // Можно передать параметр ID через QueryProperty
                await Shell.Current.GoToAsync($"{nameof(NotificationsView)}?clientId={clientId}");

                // Или просто открыть список уведомлений, как вы хотели:
                // await Shell.Current.GoToAsync(nameof(NotificationsView));
            });
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var shell = new AppShell();
            var window = new Window(shell);

            

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var token = await SecureStorage.Default.GetAsync("auth_token");

                if (!string.IsNullOrEmpty(token))
                {
                    if (!await IsTokenExpiredAsync())
                    {
                        // Токен хороший — идем внутрь и подключаемся
                        await shell.GoToAsync("//ActionPageView");
                        await _signalRService.ConnectAsync();
                    }
                    else
                    {
                      
                        SecureStorage.Default.Remove("auth_token");
                       
                        await shell.GoToAsync("//LoginPageView");
                    }

                    
                }
            });

            return window;
        }


        public static async Task<bool> IsTokenExpiredAsync()
        {

            var token = await SecureStorage.GetAsync("auth_token");

            if (string.IsNullOrEmpty(token))
            {
                return true;
            }

            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
            {
                return true;
            }

            var jwtToken = handler.ReadJwtToken(token);


            if (jwtToken.ValidTo < DateTime.UtcNow.AddSeconds(20))
            {
                return true;
            }

            return false; // Токен ещё жив
        }



        private void HandleNotification(ClientNoteNotification note)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (Application.Current?.MainPage != null)
                {
                    await Shell.Current.DisplayAlert(
                        "🔔 Новая проблема!",
                        $"Клиент: {note.ClientId}\nЗаметка: {note.NotesAboutProblems}",
                        "ОК"
                    );
                }
            });
        }

        private void HandleGlobalNotification(ClientNoteNotification note)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
#if WINDOWS
                try
                {
                    var title = "🔔 Новая проблема!";
                    var body = $"Клиент: {note.ClientId}\n{note.ClientName}\n{note.NotesAboutProblems ?? "Нет описания"}";

                    // Строим toast
                    new ToastContentBuilder()
                        .AddText(title)
                        .AddText(body)
                        // 👇 ДОБАВЛЯЕМ ДАННЫЕ ДЛЯ КЛИКА (Argument)
                        .AddArgument("action", "openClient")
                        .AddArgument("clientId", note.ClientId)
                        .Show();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка Windows-уведомления: {ex.Message}");
                }
#else
        try
        {
            // Проверка прав...
            if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            {
                bool granted = await LocalNotificationCenter.Current.RequestNotificationPermission();
                if (!granted) return;
            }

            var request = new NotificationRequest
            {
                NotificationId = Random.Shared.Next(1000, 9999),
                Title = "🔔 Новая проблема!",
                Description = $"Клиент: {note.ClientId}\n{note.NotesAboutProblems ?? "Нет описания"}",
                
                // 👇 ДОБАВЛЯЕМ ДАННЫЕ ДЛЯ КЛИКА (ReturningData)
                ReturningData = note.ClientId, 
                
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now
                }
            };

            await LocalNotificationCenter.Current.Show(request);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка уведомления: {ex.Message}");
        }
#endif
            });
        }


    }
}