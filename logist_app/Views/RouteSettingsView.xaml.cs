using logist_app.ViewModels;

namespace logist_app.Views;

public partial class RouteSettingsView : ContentPage
{

	private readonly RouteSettingsViewModel _viewModel;
    public RouteSettingsView(RouteSettingsViewModel vm)
	{
        _viewModel = vm;
        BindingContext = _viewModel;
        InitializeComponent();

	}
}