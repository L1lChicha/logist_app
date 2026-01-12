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
        // Получаем подготовленный JSON из ViewModel
        var jsonToDisplay = _viewModel.PrepareJsonForDisplay();

        if (!string.IsNullOrEmpty(jsonToDisplay))
        {
            try
            {
                // Вызываем JS на карте
                await MapWebView.EvaluateJavaScriptAsync(
                    $"displayRoute('{jsonToDisplay.Replace("'", "\\'")}')");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"JS Error: {ex.Message}", "OK");
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
            // 1. Сначала UI получает данные из WebView
            string editedGeoJson = await MapWebView.EvaluateJavaScriptAsync("window.getEditedRouteGeoJson()");

            // 2. Передаем данные во ViewModel для обработки и отправки
            await _viewModel.ConfirmRouteAsync(editedGeoJson);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось получить данные с карты: {ex.Message}", "OK");
        }
    }
}