
using logist_app.Core.Entities;
using logist_app.Infrastructure.Service;
using logist_app.ViewModels;
using System.Globalization;
using System.Threading.Tasks;

namespace logist_app.Views;

public partial class ActionPageView : ContentPage
{

    private readonly SignalRService _signalRService;
    private readonly HashSet<string> _activeDrivers = new();
    public ActionPageView(SignalRService signalRService)
    {
        InitializeComponent();
        _signalRService = signalRService;
    }



    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // 1. Подписываемся на события SignalR
       _signalRService.OnLocationReceived += HandleLocationUpdate;
        _signalRService.OnDriverDisconnected += HandleDriverDisconnect;
        // 2. Подключаемся (если еще не подключены)
        await _signalRService.ConnectAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _signalRService.OnLocationReceived -= HandleLocationUpdate;
        _signalRService.OnDriverDisconnected -= HandleDriverDisconnect; 
        // При уходе со страницы можно очистить список, так как WebView перезагрузится при возврате
        _activeDrivers.Clear();
    }



    private void HandleLocationUpdate(LocationData location)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                var id = location.DeviceId ?? "unknown_id"; // ID - это главный ключ
                var name = location.DriverName ?? "Unknown Driver";
                var plate = location.LicensePlate ?? "";

                var lat = location.Lat.ToString(CultureInfo.InvariantCulture);
                var lon = location.Lon.ToString(CultureInfo.InvariantCulture);

                if (_activeDrivers.Contains(id))
                {
                    // Водитель есть -> Двигаем по ID
                    // moveDriver(deviceId, lat, lon)
                    await MapWebView.EvaluateJavaScriptAsync($"moveDriver('{id}', {lat}, {lon})");
                }
                else
                {
                    // Водителя нет -> Добавляем
                    // addDriver(deviceId, name, lat, lon, plate)
                    // Обратите внимание на порядок аргументов, он должен совпадать с JS!
                    await MapWebView.EvaluateJavaScriptAsync($"addDriver('{id}', '{name}', {lat}, {lon}, '{plate}')");

                    _activeDrivers.Add(id);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating map: {ex.Message}");
            }
        });
    }


    private void HandleDriverDisconnect(string deviceId)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                if (_activeDrivers.Contains(deviceId))
                {
                    _activeDrivers.Remove(deviceId);
                    // Тут всё верно, удаляем по ID
                    await MapWebView.EvaluateJavaScriptAsync($"removeDriver('{deviceId}')");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing driver: {ex.Message}");
            }
        });
    }


    private async void createRoute_Clicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<RouteCreationView>();
        await Navigation.PushAsync(page);
    }

    private async void showClientsData_Clicked(object sender, EventArgs e)
    {


        var page = App.Services.GetService<ClientDataPageView>();
        await Navigation.PushAsync(page);
    }

    private async void addNewClient_Clicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<AddNewClientView>();
        await Navigation.PushAsync(page);
    }

    private async void viewRoutes_Clicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<RoutesPageView>();
        await Navigation.PushAsync(page);
    }

    private async void injectPoint_Clicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<InjectPointView>();
        await Navigation.PushAsync(page);
    }


    private async void drivers_Clicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<DriversDataView>();
        await Navigation.PushAsync(page);
    }

    private async void vehicles_Clicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<VehiclesDataView>();
        await Navigation.PushAsync(page);
    }

    private async void routeSettings_Clicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<RouteSettingsView>();
        await Navigation.PushAsync(page);
    }

    private async void appSettings_Clicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<AppSettingsView>();
        await Navigation.PushAsync(page);
    }


    private async void OnNotificationsClicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<NotificationsView>();
        await Navigation.PushAsync(page);

    }
}