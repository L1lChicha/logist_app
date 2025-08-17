using logist_app.ViewModels;

namespace logist_app;

public partial class DataViewPage : ContentPage
{
    private readonly DataViewModel _viewModel;

    public DataViewPage()
    {
        InitializeComponent();
        _viewModel = new DataViewModel();
        BindingContext = _viewModel;
    }

    public async Task RefreshClients()
    {
        await _viewModel.LoadDataAsync();
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await _viewModel.LoadDataAsync();
    }

    private async void OnClientSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ClientViewModel selectedClient)
        {
            await Navigation.PushAsync(new EditClientPage(selectedClient, RefreshClients));
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}


public class ClientViewModel
{
    public int id { get; set; }
    public string name { get; set; }
    public string address { get; set; }
    public string city { get; set; }
    public string postal_code { get; set; }
    public string phone { get; set; }
    public string email { get; set; }
    public string recurrence { get; set; }
    public int container_count { get; set; }
    public DateTime start_date { get; set; }
    public string coordinates { get; set; }
    public double lat { get; set; }
    public double lon { get; set; }
}