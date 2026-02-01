using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using logist_app.Infrastructure.Service;
using System.Net.Http.Json;
using System.Text.Json;

namespace logist_app.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SignalRService _signalRService;

    // Убрал лишние поля, объединим логику в одно свойство IsLoading
    // private bool _isAuthInProgress = false; 
    // private bool _isLoginRunning = false;

    public LoginViewModel(IHttpClientFactory httpClientFactory, SignalRService signalRService)
    {
        _httpClientFactory = httpClientFactory;
        _signalRService = signalRService;
    }

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _password;

    // ЭТО НОВОЕ СВОЙСТВО: Управляет видимостью загрузки
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotLoading))] // Чтобы отключать кнопки
    private bool _isLoading;

    // Вспомогательное свойство для блокировки кнопок (обратное от IsLoading)
    public bool IsNotLoading => !IsLoading;

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsLoading) return; // Защита от двойного клика

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Ошибка", "Введите логин и пароль", "ОК");
            return;
        }

        try
        {
            IsLoading = true; // Включаем спиннер

            var loginData = new { username = Username, password = Password };
            var client = _httpClientFactory.CreateClient("Api");

            var response = await client.PostAsJsonAsync("https://esme-aspiratory-september.ngrok-free.dev/auth/login-logistician", loginData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (result.TryGetProperty("token", out var tokenProperty))
                {
                    var token = tokenProperty.GetString();
                    await SecureStorage.Default.SetAsync("auth_token", token);

                    // Передаем управление методу навигации (он сам выключит IsLoading в конце)
                    await ConnectAndNavigateAsync();
                    return; // Важно выйти, чтобы finally внизу не сработал раньше времени, 
                            // либо убрать IsLoading = false отсюда и оставить только в ConnectAndNavigateAsync
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Ошибка", "Неверный логин или пароль", "ОК");
                IsLoading = false; // Выключаем, если ошибка
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Ошибка сети", ex.Message, "ОК");
            IsLoading = false; // Выключаем при ошибке
        }
    }

    [RelayCommand]
    private async Task BiometricLoginAsync()
    {
        if (IsLoading) return;

        // ОБЯЗАТЕЛЬНО: Запускаем в главном потоке UI
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            IsLoading = true;

#if WINDOWS
            var token = await SecureStorage.Default.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token))
            {
                await Shell.Current.DisplayAlert("Внимание", "Для первого входа используйте логин и пароль.", "ОК");
                IsLoading = false;
                return;
            }

            try
            {
                // Дополнительная проверка: активно ли окно сейчас?
                // Если приложение в фоне, UserConsentVerifier кинет исключение или вернет ошибку
                var availability = await Windows.Security.Credentials.UI.UserConsentVerifier.CheckAvailabilityAsync();

                if (availability != Windows.Security.Credentials.UI.UserConsentVerifierAvailability.Available)
                {
                    // Если недоступно (например, окно не в фокусе), просто выключаем загрузку
                    // Не показываем ошибку пользователю, если это авто-запуск, чтобы не пугать его
                    IsLoading = false;
                    return;
                }

                var result = await Windows.Security.Credentials.UI.UserConsentVerifier.RequestVerificationAsync("Подтвердите вход в Logist App");

                if (result == Windows.Security.Credentials.UI.UserConsentVerificationResult.Verified)
                {
                    await ConnectAndNavigateAsync();
                }
                else
                {
                    IsLoading = false;
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не всегда показываем алерт, если это авто-вход
                System.Diagnostics.Debug.WriteLine($"Windows Hello Error: {ex.Message}");
                IsLoading = false;
            }
#else
        await Shell.Current.DisplayAlert("Инфо", "Только для Windows", "ОК");
        IsLoading = false;
#endif
        });
    }

    public async Task CheckAutoLoginAsync()
    {
        bool isExpired = await App.IsTokenExpiredAsync();
        if (!isExpired)
        {
            await BiometricLoginAsync();
        }
        else
        {
            SecureStorage.Default.Remove("auth_token");
        }
    }

    private async Task ConnectAndNavigateAsync()
    {
        // Убеждаемся, что статус загрузки включен (на случай вызова из разных мест)
        IsLoading = true;

        try
        {
            // Здесь происходит основная задержка (подключение к сокетам)
            await _signalRService.ConnectAsync();

            // Переход на новую страницу
            await Shell.Current.GoToAsync("//ActionPageView");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Ошибка подключения", $"Не удалось соединиться с сервером: {ex.Message}", "ОК");
        }
        finally
        {
            // Выключаем загрузку в самом конце, даже если произошла ошибка
            IsLoading = false;
        }
    }
}