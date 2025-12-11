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
}