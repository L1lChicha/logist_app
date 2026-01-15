
using System.Threading.Tasks;

namespace logist_app.Views;

public partial class ActionPageView : ContentPage
{
    public ActionPageView()
    {
        InitializeComponent();
    }

    private async void createRoute_Clicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<RouteCreationView>();
        await Navigation.PushAsync(page);
    }

    private async void showClientsData_Clicked(object sender, EventArgs e)
    {


        var page = App.Services.GetService<ClientDataPageView>();
        await Navigation.PushAsync(page);
    }

    private async void addNewClient_Clicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<AddNewClientView>();
        await Navigation.PushAsync(page);
    }

    private async void viewRoutes_Clicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<RoutesPageView>();
        await Navigation.PushAsync(page);
    }


    private async void drivers_Clicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<DriversDataView>();
        await Navigation.PushAsync(page);
    }

    private async void vehicles_Clicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<VehiclesDataView>();
        await Navigation.PushAsync(page);
    }

    private async void routeSettings_Clicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<RouteSettingsView>();
        await Navigation.PushAsync(page);
    }

    private async void appSettings_Clicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<AppSettingsView>();
        await Navigation.PushAsync(page);
    }


    private async void OnNotificationsClicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<NotificationsView>();
        await Navigation.PushAsync(page);

    }

   

}