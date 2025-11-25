
namespace logist_app.Views;

public partial class ViewRoutesPage : ContentPage
{
    private RoutesListViewModel ViewModel => BindingContext as RoutesListViewModel;
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

    private async void RefreshButton_Clicked(object sender, EventArgs e)
    {
        await ViewModel.LoadRoutesAsync();
    }

    private async void OnRouteSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Route selectedRoute)
        {
            Route routeToShow = selectedRoute;

        
            if (string.IsNullOrEmpty(selectedRoute.GeometryJson))
            {
                var fullRoute = await _vm.GetRouteByIdAsync(selectedRoute.Id);
                if (fullRoute == null)
                {
                    await DisplayAlert("Ошибка", "Не удалось загрузить маршрут", "OK");
                    return;
                }
                routeToShow = fullRoute;
            }

            if (string.IsNullOrEmpty(routeToShow.GeometryJson))
            {
                await DisplayAlert("Ошибка", "Маршрут не содержит данных геометрии.", "OK");
                return;
            }

            await Navigation.PushAsync(new AcceptRouteView(routeToShow.GeometryJson, routeToShow.Id));
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}