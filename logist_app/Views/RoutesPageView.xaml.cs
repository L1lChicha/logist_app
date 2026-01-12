
namespace logist_app.Views;

public partial class RoutesPageView : ContentPage
{  
    private readonly RoutesListViewModel _vm;
    public RoutesPageView(RoutesListViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadRoutesAsync();
    }

}