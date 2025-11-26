using logist_app.Models;
using logist_app.ViewModels;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;

namespace logist_app.Views;

public partial class RouteCreationPage : ContentPage
{
    private readonly ClientDataViewModel _viewModel;
    private readonly ApiSettings _apiSettings;
    private readonly IHttpClientFactory _httpClientFactory;

    private List<int> selectedClientIds = new();

    // ✅ получаем зависимости через DI
    public RouteCreationPage(ClientDataViewModel viewModel, ApiSettings apiSettings, IHttpClientFactory httpClientFactory)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _apiSettings = apiSettings;
        _httpClientFactory = httpClientFactory;

        BindingContext = _viewModel;
        createRouteButton.IsEnabled = false;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDataAsync();
    }

    public async Task RefreshClients() => await _viewModel.LoadDataAsync();

    private async void OnRefreshClicked(object sender, EventArgs e) =>
        await _viewModel.LoadDataAsync();

    private void OnClientsSelected(object sender, SelectionChangedEventArgs e)
    {
        foreach (var added in e.CurrentSelection.OfType<ClientViewModel>())
            added.IsSelected = true;

        foreach (var removed in e.PreviousSelection.OfType<ClientViewModel>()
                                                   .Except(e.CurrentSelection.OfType<ClientViewModel>()))
            removed.IsSelected = false;

        var selected = e.CurrentSelection.OfType<ClientViewModel>().ToList();
        selectedClientIds = selected.Select(c => c.Id).ToList();
        createRouteButton.IsEnabled = selected.Count > 1;
        if (createRouteButton.IsEnabled)
        {
            createRouteButton.BackgroundColor = Color.FromArgb("#62b375");
        }
        else
        {
            createRouteButton.BackgroundColor = Color.FromArgb("#404040");
        }
        
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

                var routeData = json.RootElement.GetProperty("route_data").ToString();
                var routeId = int.Parse(json.RootElement.GetProperty("route_id").ToString());

                await DisplayAlert("Success", "Маршрут успешно построен", "OK");
                await Navigation.PushAsync(new AcceptRouteView(routeData, routeId));

                clientsCollectionView.SelectedItems.Clear();
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
