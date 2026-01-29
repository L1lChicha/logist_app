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
    private bool _isAuthInProgress = false;
    private bool _isLoginRunning = false;
    public LoginViewModel(IHttpClientFactory httpClientFactory, SignalRService signalRService)
    {
        _httpClientFactory = httpClientFactory;
        _signalRService = signalRService;
    }

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _password;

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Ошибка", "Введите логин и пароль", "ОК");
            return;
        }

        try
        {
            var loginData = new { username = Username, password = Password };
            var client = _httpClientFactory.CreateClient("Api");

         
            //var response = await client.PostAsJsonAsync("https://esme-aspiratory-september.ngrok-free.dev/auth/login-logistician", loginData);

            var response = await client.PostAsJsonAsync("https://localhost:777/auth/login-logistician", loginData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (result.TryGetProperty("token", out var tokenProperty))
                {
                    var token = tokenProperty.GetString();

                    await SecureStorage.Default.SetAsync("auth_token", token);

                    await ConnectAndNavigateAsync();
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

    
      
        [RelayCommand]
        private async Task BiometricLoginAsync()
        {
           
            if (_isLoginRunning) return;

           
            _isLoginRunning = true;

#if WINDOWS
            
            var token = await SecureStorage.Default.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token))
            {
                await Shell.Current.DisplayAlert("Внимание", "Для первого входа используйте логин и пароль.", "ОК");
                _isLoginRunning = false; 
                return;
            }

            try
            {
                var availability = await Windows.Security.Credentials.UI.UserConsentVerifier.CheckAvailabilityAsync();

                if (availability != Windows.Security.Credentials.UI.UserConsentVerifierAvailability.Available)
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Windows Hello не настроен.", "ОК");
                    _isLoginRunning = false; 
                    return;
                }

                var result = await Windows.Security.Credentials.UI.UserConsentVerifier.RequestVerificationAsync("Подтвердите вход в Logist App");

                if (result == Windows.Security.Credentials.UI.UserConsentVerificationResult.Verified)
                {
                   
                    await ConnectAndNavigateAsync();
                }
                else
                {
                 
                    _isLoginRunning = false;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка Windows Hello", ex.Message, "ОК");
                _isLoginRunning = false; 
            }
#else
        await Shell.Current.DisplayAlert("Инфо", "Только для Windows", "ОК");
        _isLoginRunning = false;
#endif
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
        try
        {
            await _signalRService.ConnectAsync();

            await Shell.Current.GoToAsync("//ActionPageView");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Ошибка подключения", $"Не удалось соединиться с сервером: {ex.Message}", "ОК");
        }
    }
}