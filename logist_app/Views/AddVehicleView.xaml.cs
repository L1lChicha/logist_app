using logist_app.ViewModels;

namespace logist_app.Views;

public partial class AddVehicleView: ContentPage
{
    public AddVehicleView(AddVehicleViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}