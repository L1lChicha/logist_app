namespace logist_app.Views;

public partial class DriverManagerView : ContentPage
{
	public DriverManagerView()
	{
		InitializeComponent();
	}

    private async void AddNewDriverButton_Clicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<AddNewDriverPage>();
        await Navigation.PushAsync(page);
    }

    private async void ShowAllDriversButton_Clicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<DriversDataView>();
        await Navigation.PushAsync(page);
    }
}