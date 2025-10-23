using logist_app.Factories;
using logist_app.Models;
using logist_app.ViewModels;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace logist_app;

public partial class DataViewPage : ContentPage
{
    private readonly DataViewModel _viewModel;

    public DataViewPage(DataViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public async Task RefreshClients() => await _viewModel.LoadDataAsync();

    private async void OnRefreshClicked(object sender, EventArgs e) =>
        await _viewModel.LoadDataAsync();

    private async void OnClientSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ClientViewModel selectedClient)
        {
            var factory = App.Services.GetService<IEditClientVmFactory>();
            if (factory is null)
            {
                await DisplayAlert("Error", "EditClientVmFactory is not registered.", "OK");
                return;
            }

            await Navigation.PushAsync(new EditClientPage(selectedClient, RefreshClients, factory));
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}
public class ClientViewModel
{
    [JsonPropertyName("id")] public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    [JsonPropertyName("postal_code")] public string? PostalCode { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Recurrence { get; set; }
    [JsonPropertyName("container_count")] public int? ContainerCount { get; set; }
    [JsonPropertyName("start_date")] public DateTime? StartDate { get; set; }
    public string Coordinates { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
    bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set { if (_isSelected == value) return; _isSelected = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string n = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));



}