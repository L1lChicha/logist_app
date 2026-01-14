using logist_app.ViewModels;

namespace logist_app.Views;

public partial class AppSettingsView : ContentPage
{
	public AppSettingsView(AppSettingsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}