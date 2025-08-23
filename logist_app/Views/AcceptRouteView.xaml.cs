using logist_app.ViewModels;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace logist_app.Views;
public partial class AcceptRouteView : ContentPage
{
   private string route;
    private int id;
	public AcceptRouteView(string routeGeoJson, int routeId)
	{
		InitializeComponent();
        route = routeGeoJson; 
        id = routeId;
    }
	  private async void MapWebView_Navigated(object sender, WebNavigatedEventArgs e)
    {
      
        await MapWebView.EvaluateJavaScriptAsync($"displayRoute('{route.Replace("'", "\\'")}')");
    }

    private async void ShowRouteButton_Clicked(object sender, EventArgs e)
    {
     
        string jsCode = $"displayRoute('{route.Replace("'", "\\'").Replace("\n", "")}');";

        await MapWebView.EvaluateJavaScriptAsync(jsCode);
        await MapWebView.EvaluateJavaScriptAsync("map.invalidateSize();");

    }

    private async void confirmRoute_Clicked(object sender, EventArgs e)
    {
        try
        {
            var ApiUrl = $"https://localhost:32769/api/Route/confirm/{id}";
            using var httpClient = new HttpClient();
            
            var response = await httpClient.PostAsync(ApiUrl,null);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Готово", "Маршрут подтвержден!", "OK");
                await Navigation.PopToRootAsync();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Ошибка", error, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось выполнить запрос: {ex.Message}", "OK");
        }
    }

    private async void rejectRoute_Clicked(object sender, EventArgs e)
    {
        try
        {
            var ApiUrl = $"https://localhost:32769/api/Route/reject/{id}";
            using var httpClient = new HttpClient();

            var response = await httpClient.PostAsync(ApiUrl, null);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Готово", "Маршрут отклонен!", "OK");
                await Navigation.PopToRootAsync();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Ошибка", error, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось выполнить запрос: {ex.Message}", "OK");
        }
    }
}