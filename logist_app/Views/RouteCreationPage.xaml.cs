using logist_app.ViewModels;
using logist_app.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace logist_app.Views;

public partial class RouteCreationPage : ContentPage
{
    private readonly DataViewModel _viewModel;
    private readonly ApiSettings _apiSettings;
    private readonly IHttpClientFactory _httpClientFactory;

    private List<int> selectedClientIds = new();

    // ✅ получаем зависимости через DI
    public RouteCreationPage(DataViewModel viewModel, ApiSettings apiSettings, IHttpClientFactory httpClientFactory)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _apiSettings = apiSettings;
        _httpClientFactory = httpClientFactory;

        BindingContext = _viewModel;
        createRouteButton.IsEnabled = false;
    }

    public async Task RefreshClients() => await _viewModel.LoadDataAsync();

    private async void OnRefreshClicked(object sender, EventArgs e) =>
        await _viewModel.LoadDataAsync();

    private void OnClientsSelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedClients = e.CurrentSelection.Cast<ClientViewModel>().ToList();
        selectedClientIds = selectedClients.Select(c => c.Id).ToList();
        createRouteButton.IsEnabled = selectedClients.Count > 1;
    }

    private async void createRouteButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            var http = _httpClientFactory.CreateClient("Api");

            // Можно отправлять относительным путём (база уже задана в HttpClient)
            var response = await http.PostAsJsonAsync(_apiSettings.RoutesBuildEndpoint, selectedClientIds);

            if (response.IsSuccessStatusCode)
            {
                using var stream = await response.Content.ReadAsStreamAsync();
                using var json = await JsonDocument.ParseAsync(stream);

                var routeData = json.RootElement.GetProperty("routeData").ToString();
                var routeId = int.Parse(json.RootElement.GetProperty("routeId").ToString());

                await DisplayAlert("Success", "Маршрут успешно построен", "OK");
                await Navigation.PushAsync(new AcceptRouteView(routeData, routeId));
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Error", $"{response.StatusCode}\n{error}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
