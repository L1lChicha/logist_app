using logist_app.Core.Entities;
using logist_app.Models;
using logist_app.ViewModels;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace logist_app.Views;

public partial class AddNewDriverPage : ContentPage
{
	public AddNewDriverPage()
	{
		InitializeComponent();
	}

    private async void createNewDriverButton_Clicked(object sender, EventArgs e)
    {
		string name = nameEntry.Text;
		string phoneNumber = phoneEntry.Text;

		if (name == "" || phoneNumber == "")
		{
			await DisplayAlert("Error","Put correct data","OK");
		}
		else
		{
            var newDriver = new Driver();
            newDriver.Name = name;
            newDriver.PhoneNumber = phoneNumber;
            newDriver.CreatedAt= DateTime.UtcNow;
            var success = await AddNewDriverAsync(newDriver);
            if (success)
            {

                await DisplayAlert("Success", "Driver added successfully.", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Failed to add driver.", "OK");
            }

        }


    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync(); // Или PopModalAsync, смотря как открывали
    }
    private async Task<bool> AddNewDriverAsync(Driver newDriver)
    {
        try
        {


            var api = App.Services.GetRequiredService<ApiSettings>();
            var httpFactory = App.Services.GetRequiredService<IHttpClientFactory>();
            var http = httpFactory.CreateClient("Api"); // BaseAddress уже настроен в MauiProgram
            var token = await SecureStorage.Default.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            // отправляем на относительный endpoint из конфигурации
            var response = await http.PostAsJsonAsync(api.DriversEndpoint, newDriver);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding client: {ex.Message}");
            return false;
        }
    }
}