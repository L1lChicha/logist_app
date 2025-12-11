using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input; // Важно для команд
using logist_app.Infrastructure.Service;
using System.Net.Http.Json;
using System.Text.Json;

namespace logist_app.ViewModels;

// 1. Обязательно partial!
public partial class LoginViewModel : ObservableObject
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SignalRService _signalRService;

    public LoginViewModel(IHttpClientFactory httpClientFactory, SignalRService signalRService)
    {
        _httpClientFactory = httpClientFactory;
        _signalRService = signalRService;
    }

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _password;

    // 2. Атрибут RelayCommand автоматически создает команду "LoginCommand"
    [RelayCommand]
    private async Task LoginAsync()
    {
        // Проверка на пустоту
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Ошибка", "Введите логин и пароль", "ОК");
            return;
        }

        try
        {
            // Берем данные из свойств класса (Username и Password созданы генератором)
            var loginData = new { username = Username, password = Password };

            var client = _httpClientFactory.CreateClient("Api");

            // 3. Стучимся на сервер
            var response = await client.PostAsJsonAsync("https://esme-aspiratory-september.ngrok-free.dev/auth/login-logistician", loginData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<JsonElement>();

                // Проверяем, есть ли свойство token
                if (result.TryGetProperty("token", out var tokenProperty))
                {
                    var token = tokenProperty.GetString();

                    // 4. Сохраняем токен
                    await SecureStorage.Default.SetAsync("auth_token", token);

                    // 5. Запускаем SignalR
                    await _signalRService.ConnectAsync();

                    // 6. Переходим на главный экран
                    // Убедись, что маршрут "//ActionPageView" правильный и зарегистрирован в AppShell
                    Application.Current.MainPage = new AppShell();
                    await Shell.Current.GoToAsync("//ActionPageView");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Ошибка", "Неверный логин или пароль", "ОК");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Ошибка сети", ex.Message, "ОК");
        }
    }
}