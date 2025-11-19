
using System.Threading.Tasks;

namespace logist_app.Views;

public partial class ActionPage : ContentPage
{
	public ActionPage()
	{
		InitializeComponent();
	}

    private async void createRoute_Clicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<RouteCreationPage>();
        await Navigation.PushAsync(page);
    }

    private async void showClientsData_Clicked(object sender, EventArgs e)
    {


        var page = App.Services.GetService<ClientDataPageView>();
        await Navigation.PushAsync(page);
    }

    private void addNewClient_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MainPage());
    }

    private async void viewRoutes_Clicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<ViewRoutesPage>();
        await Navigation.PushAsync(page);
    }

   
    private async void drivers_Clicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<DriverManagerView>();
        await Navigation.PushAsync(page);
    }
}