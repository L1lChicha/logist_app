using logist_app.ViewModels;

namespace logist_app.Views;

public partial class LoginPageView : ContentPage
{
    private bool _isFirstLoad = true;
    // Внедряем ViewModel через конструктор
    public LoginPageView(LoginViewModel viewModel)
    {
        InitializeComponent();

        // Устанавливаем контекст данных
        BindingContext = viewModel;
    }

#if WINDOWS
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_isFirstLoad)
        {
            _isFirstLoad = false;

            await Task.Delay(200);

            if (BindingContext is LoginViewModel vm)
            {
                await vm.CheckAutoLoginAsync();
            }
        }
    }
#endif
}