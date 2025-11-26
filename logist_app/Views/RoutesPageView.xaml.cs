
namespace logist_app.Views;

public partial class ViewRoutesPage : ContentPage
{  
    private readonly RoutesListViewModel _vm;
    public ViewRoutesPage(RoutesListViewModel vm)
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