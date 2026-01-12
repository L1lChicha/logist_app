using logist_app.ViewModels;

namespace logist_app.Views;

public partial class VehiclesDataView : ContentPage
{

	private readonly VehiclesDataViewModel _viewModel;
    public VehiclesDataView(VehiclesDataViewModel viewModel)
	{
		_viewModel = viewModel;
        InitializeComponent();
        BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadVehiclesAsync();
    }
}