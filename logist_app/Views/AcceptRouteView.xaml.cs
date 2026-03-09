using logist_app.Core.Entities;
using logist_app.ViewModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace logist_app.Views;

public partial class AcceptRouteView : ContentPage
{
    private readonly AcceptRouteViewModel _viewModel;
    private List<RoutePoint> _routePoints;
   
    public AcceptRouteView(string routeGeoJson, List<RoutePoint> routePoints, int routeId, string routeName)
    {
        InitializeComponent();

        if (string.IsNullOrEmpty(routeGeoJson))
            throw new ArgumentNullException(nameof(routeGeoJson));
        if (routeId <= 0)
            throw new ArgumentException("Id должен быть > 0", nameof(routeId));

        _viewModel = App.Services.GetRequiredService<AcceptRouteViewModel>();
        
        _viewModel.Initialize(routeGeoJson, routePoints, routeId, routeName);
        _routePoints = routePoints;
        BindingContext = _viewModel;
    }

    private async void MapWebView_Navigated(object sender, WebNavigatedEventArgs e)
    {
        // 1. Сначала рисуем саму линию маршрута
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
                await DisplayAlert("Ошибка маршрута", $"JS Error: {ex.Message}", "OK");
            }
        }

        // 2. ЗАТЕМ ОТДЕЛЬНО рисуем точки (независимо от того, нарисовалась ли линия)
        await DrawRoutePointsOnMapAsync(_routePoints);
    }


    public async Task DrawRoutePointsOnMapAsync(List<RoutePoint> points)
    {
        if (points == null || !points.Any()) return;

        var options = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        string rawJson = JsonSerializer.Serialize(points, options);

        // 1. КОДИРУЕМ В BASE64 (Никаких слэшей и кавычек!)
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(rawJson);
        string base64Json = Convert.ToBase64String(plainTextBytes);

        // 2. Вызываем специальную новую функцию в JS и передаем Base64 как обычную строку
        string script = $"displayRoutePointsBase64('{base64Json}');";

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                await MapWebView.EvaluateJavaScriptAsync(script);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка C#", $"Не удалось отправить скрипт: {ex.Message}", "OK");
            }
        });
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