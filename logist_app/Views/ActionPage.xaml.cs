
namespace logist_app.Views;

public partial class ActionPage : ContentPage
{
	public ActionPage()
	{
		InitializeComponent();
	}

    private void createRoute_Clicked(object sender, EventArgs e)
    {
         Navigation.PushAsync(new RouteCreationPage());
    }

    private void showClientsData_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new DataViewPage());
    }

    private void addNewClient_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MainPage());
    }
}