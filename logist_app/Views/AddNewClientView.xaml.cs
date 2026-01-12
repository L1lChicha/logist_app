using logist_app.ViewModels;
using logist_app.Views; // For RecurrenceModalPage
using System.Globalization;
using System.Text.Json;

namespace logist_app.Views;

public partial class AddNewClientView : ContentPage
{
    private readonly AddNewClientViewModel _viewModel;

    public AddNewClientView(AddNewClientViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        MapWebView.Source = "map.html";
    }

    // WebView interaction usually stays in CodeBehind or requires a service
    private async void GetLocationData_Clicked(object sender, EventArgs e)
    {
        try
        {
            var json = await MapWebView.EvaluateJavaScriptAsync("getSelectedLocation()");
            json = json.Trim('"').Replace("\\", "");
            var data = JsonSerializer.Deserialize<MapData>(json);

            if (data != null)
            {
                _viewModel.Lat = data.lat;
                _viewModel.Lon = data.lon;

                string[] addressDetails = data.address.Split(',');

                // Simple address parsing logic from your original code
                if (addressDetails.Length > 6 && addressDetails[4].Trim() == "Брест")
                {
                    _viewModel.Address = addressDetails[1].Trim() + ", " + addressDetails[0];
                    _viewModel.City = addressDetails[4].Trim();
                    _viewModel.PostalCode = addressDetails[6].Trim();
                }
                else
                {
                    // Fallback or full address if format differs
                    _viewModel.Address = data.address;
                }
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Failed to get location from map.", "OK");
        }
    }

    private async void FindButton_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(_viewModel.Address))
        {
            var result = await _viewModel.GeocodeAddressAsync(_viewModel.Address);
            if (result is (double lat, double lon, string displayName))
            {
                _viewModel.Lat = lat;
                _viewModel.Lon = lon;
                string jsCode = $"setMarker({lat.ToString(CultureInfo.InvariantCulture)}, {lon.ToString(CultureInfo.InvariantCulture)}, '{displayName.Replace("'", "\\'")}');";
                await MapWebView.EvaluateJavaScriptAsync(jsCode);
            }
        }
        else
        {
            await DisplayAlert("Error", "The address field must be filled", "OK");
        }
    }

    private async void OnViewDataClicked(object sender, EventArgs e)
    {
        // Assuming ClientDataPageView is registered in DI
        var page = Handler.MauiContext.Services.GetService<ClientDataPageView>();
        if (page != null) await Navigation.PushAsync(page);
    }

    private async void OnRecurrenceConfigClicked(object sender, EventArgs e)
    {
        var modalPage = new RecurrenceModalPage(_viewModel.RecurrenceSettings, (newSettings) =>
        {
            _viewModel.RecurrenceSettings = newSettings;
            _viewModel.UpdateRecurrenceSummary();
        });
        await Navigation.PushModalAsync(modalPage);
    }

    public class MapData
    {
        public double lat { get; set; }
        public double lon { get; set; }
        public string address { get; set; }
    }
}