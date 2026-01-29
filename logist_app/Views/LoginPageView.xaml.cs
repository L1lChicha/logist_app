using logist_app.ViewModels;

namespace logist_app.Views;

public partial class LoginPageView : ContentPage
{
    // Внедряем ViewModel через конструктор
    public LoginPageView(LoginViewModel viewModel)
    {
        InitializeComponent();

        // Устанавливаем контекст данных
        BindingContext = viewModel;
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is LoginViewModel vm)
        {
            // Небольшая задержка важна для Windows!
            // Окну нужно время, чтобы отрисоваться и получить дескриптор (Handle),
            // иначе UserConsentVerifier может выдать ошибку или не показаться.
            await Task.Delay(200);

            // Запускаем проверку токена и (если ок) биометрию
            await vm.CheckAutoLoginAsync();
        }
    }
}