
using CommunityToolkit.Mvvm.ComponentModel;
using logist_app.ViewModels;

namespace logist_app.Views;

public partial class DriversDataView : ContentPage
{
    
    private readonly DriversViewModel _vm;
   
    public DriversDataView(DriversViewModel vm)
	{
		InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadDriversAsync();
    }


}