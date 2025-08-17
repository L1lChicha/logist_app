using logist_app.ViewModels;
namespace logist_app.Views;

using CommunityToolkit.Maui.Core.Views;
using System.Net.Http.Json;


public partial class RouteCreationPage : ContentPage
{
    private const string ApiUrl = "https://localhost:32771/api/Route";
    private readonly DataViewModel _viewModel;
    private List<int> selectedClientIds = new List<int>();

    public RouteCreationPage()
	{
		InitializeComponent();
        _viewModel = new DataViewModel();
        BindingContext = _viewModel;
        createRouteButton.IsEnabled = false;
    }
    public async Task RefreshClients()
    {
        await _viewModel.LoadDataAsync();
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await _viewModel.LoadDataAsync();
        
    }

    private void OnClientsSelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedClients = e.CurrentSelection.Cast<ClientViewModel>().ToList();
        selectedClientIds = selectedClients.Select(c => c.id).ToList();

        if (selectedClients.Count > 1) 
        {
            createRouteButton.IsEnabled = true;
        }
        else
        {
            createRouteButton.IsEnabled = false;
        }


    }

    private async void createRouteButton_Clicked(object sender, EventArgs e)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsJsonAsync(ApiUrl, selectedClientIds);

        if (response.IsSuccessStatusCode)
        {
            var routeGeoJson = await response.Content.ReadAsStringAsync();
            await DisplayAlert("Success", "Маршрут успешно построен", "OK");
            // await MapWebView.EvaluateJavaScriptAsync($"displayRoute('{routeGeoJson.Replace("'", "\\'")}');");
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Ошибка при получении маршрута: {response.StatusCode} {error}");
            await DisplayAlert("Error", $"{response.StatusCode}\n{error}", "OK");
        }
    }
}