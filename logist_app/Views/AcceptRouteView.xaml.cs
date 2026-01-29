using logist_app.ViewModels;

namespace logist_app.Views;

public partial class AcceptRouteView : ContentPage
{
    private readonly AcceptRouteViewModel _viewModel;

   
    public AcceptRouteView(string routeGeoJson, int routeId)
    {
        InitializeComponent();

        if (string.IsNullOrEmpty(routeGeoJson))
            throw new ArgumentNullException(nameof(routeGeoJson));
        if (routeId <= 0)
            throw new ArgumentException("Id должен быть > 0", nameof(routeId));

        // Получаем ViewModel из DI контейнера
        _viewModel = App.Services.GetRequiredService<AcceptRouteViewModel>();

        // Инициализируем ViewModel данными
        _viewModel.Initialize(routeGeoJson, routeId);

        // Устанавливаем контекст данных
        BindingContext = _viewModel;
    }

    private async void MapWebView_Navigated(object sender, WebNavigatedEventArgs e)
    {
        var jsonToDisplay = _viewModel.PrepareJsonForDisplay();

        if (!string.IsNullOrEmpty(jsonToDisplay))
        {
            try
            {
                await MapWebView.EvaluateJavaScriptAsync(
                    $"displayRoute('{jsonToDisplay.Replace("'", "\\'")}')");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"JS Error: {ex.Message}", "OK");
            }
        }
    }

    private async void EditRoute_Clicked(object sender, EventArgs e)
    {
        // UI логика: переключение режима карты
        await MapWebView.EvaluateJavaScriptAsync("map.pm.toggleGlobalEditMode();");
    }

    private async void ConfirmRoute_Clicked(object sender, EventArgs e)
    {
        try
        {
            string editedGeoJson = await MapWebView.EvaluateJavaScriptAsync("window.getEditedRouteGeoJson()");

            // 2. Передаем данные во ViewModel для обработки и отправки
            await _viewModel.ConfirmRouteAsync(editedGeoJson);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to retrieve data from the card: {ex.Message}", "OK");
        }
    }
}