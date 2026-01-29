using logist_app.Core.Entities;
using logist_app.Infrastructure.Service;
using logist_app.Services;
using logist_app.ViewModels;
using logist_app.Views;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices; // <--- НУЖНО ДЛЯ DllImport

#if WINDOWS
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
#endif

namespace logist_app
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }
        private readonly SignalRService _signalRService;
        private readonly NotificationService _notificationService;

        // --- WIN32 API ДЛЯ УПРАВЛЕНИЯ ОКНОМ ---
#if WINDOWS
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int SW_MAXIMIZE = 3;
#endif
        // ---------------------------------------

        public App(IServiceProvider serviceProvider, SignalRService signalRService, NotificationService notificationService)
        {
            InitializeComponent();

            Services = serviceProvider;
            _signalRService = signalRService;
            _notificationService = notificationService;

            LoadSavedTheme();

            _signalRService.OnNoteReceived += HandleIncomingNote;

#if WINDOWS
            ToastNotificationManagerCompat.OnActivated -= OnWindowsToastActivated;
            ToastNotificationManagerCompat.OnActivated += OnWindowsToastActivated;
#endif
        }

        private void LoadSavedTheme()
        {
            var savedTheme = Preferences.Default.Get("app_theme", "Light");
            AppSettingsViewModel.ApplyTheme(savedTheme);
        }

        private void HandleIncomingNote(ClientNoteNotification note)
        {
            _notificationService.Add(
                $"Проблема: {note.ClientId}, {note.ClientName}",
                note.NotesAboutProblems ?? "Нет описания"
            );

            ShowWindowsToast(note);
        }

        private void ShowWindowsToast(ClientNoteNotification note)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
#if WINDOWS
                try
                {
                    new ToastContentBuilder()
                        .AddText("🔔 New problem!")
                        .AddText($"Client: {note.ClientId}\n{note.ClientName}")
                        .AddArgument("action", "openClient")
                        .AddArgument("clientId", note.ClientId)
                        .Show();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                }
#endif
            });
        }

#if WINDOWS
        private void OnWindowsToastActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            var args = ToastArguments.Parse(e.Argument);

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var window = Application.Current?.Windows.FirstOrDefault();
                if (window != null)
                {
                    var nativeWindow = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
                    if (nativeWindow != null)
                    {
                        var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);

                        ShowWindow(windowHandle, SW_MAXIMIZE);

                        SetForegroundWindow(windowHandle);
                    }
                }
                if (args.TryGetValue("action", out string action) && action == "openClient")
                {
                    if (args.TryGetValue("clientId", out string clientId))
                    {
                        await Shell.Current.GoToAsync($"{nameof(NotificationsView)}?clientId={clientId}");
                    }
                }
                else
                {
                    await Shell.Current.GoToAsync(nameof(NotificationsView));
                }
            });
        }
#endif

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
            if (string.IsNullOrEmpty(token)) return true;
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token)) return true;
            var jwtToken = handler.ReadJwtToken(token);
            if (jwtToken.ValidTo < DateTime.UtcNow.AddSeconds(20)) return true;
            return false;
        }


    }
}