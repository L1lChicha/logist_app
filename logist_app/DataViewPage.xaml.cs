using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace logist_app;

public partial class DataViewPage : ContentPage
{
    private readonly ObservableCollection<ClientViewModel> Clients = new ObservableCollection<ClientViewModel>();
    private const string ApiUrl = "https://localhost:32769/api/Clients"; // Замените на ваш URL API

    public DataViewPage()
    {
        InitializeComponent();
        LoadDataAsync();
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            Clients.Clear();
            Console.WriteLine("Cleared Clients collection");

            var clients = await FetchClientsFromApiAsync();

            if (clients.Count > 0)
            {
                foreach (var client in clients)
                {
                    Clients.Add(client);
                }

                UpdateUI();
                await DisplayAlert("Success", $"Uploaded records: {clients.Count}.", "OK");
            }
            else
            {
                DisplayNoDataMessage();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            await DisplayAlert("Error", $"Error loading data: {ex.Message}", "OK");
        }
    }

    //получение данных
    private async Task<ObservableCollection<ClientViewModel>> FetchClientsFromApiAsync()
    {
        using var httpClient = new HttpClient();

            var response = await httpClient.GetAsync(ApiUrl);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to load data. Status code: {response.StatusCode}");
        }

        var clients = await response.Content.ReadFromJsonAsync<ObservableCollection<ClientViewModel>>();
        return clients ?? new ObservableCollection<ClientViewModel>();
    }

    private void UpdateUI()
    {
        ClientsStackLayout.Children.Clear();

        foreach (var client in Clients)
        {
            var frame = new Frame
            {
                BorderColor = Color.FromArgb("#CCCCCC"),
                CornerRadius = 5,
                Margin = new Thickness(5),
                Padding = new Thickness(10)
            };

            var grid = new Grid
            {
                RowDefinitions =
        {
            new RowDefinition { Height = GridLength.Auto },
            new RowDefinition { Height = GridLength.Auto },
            new RowDefinition { Height = GridLength.Auto },
            new RowDefinition { Height = GridLength.Auto },
            new RowDefinition { Height = GridLength.Auto },
            new RowDefinition { Height = GridLength.Auto },
            new RowDefinition { Height = GridLength.Auto },
            new RowDefinition { Height = GridLength.Auto },
            new RowDefinition { Height = GridLength.Auto },
            new RowDefinition { Height = GridLength.Auto }
        },
                ColumnDefinitions =
        {
            new ColumnDefinition { Width = GridLength.Auto },
            new ColumnDefinition { Width = GridLength.Star }
        },
                RowSpacing = 5
            };

            grid.Add(new Label { Text = "ID:", FontSize = 12, FontAttributes = FontAttributes.Bold }, 0, 0);
            grid.Add(new Label { Text = client.id.ToString(), FontSize = 12 }, 1, 0);

            grid.Add(new Label { Text = "Name:", FontSize = 12, FontAttributes = FontAttributes.Bold }, 0, 1);
            grid.Add(new Label { Text = client.name, FontSize = 12 }, 1, 1);

            grid.Add(new Label { Text = "Address:", FontSize = 12, FontAttributes = FontAttributes.Bold }, 0, 2);
            grid.Add(new Label { Text = client.address, FontSize = 12 }, 1, 2);

            grid.Add(new Label { Text = "City:", FontSize = 12, FontAttributes = FontAttributes.Bold }, 0, 3);
            grid.Add(new Label { Text = client.city, FontSize = 12 }, 1, 3);

            grid.Add(new Label { Text = "Post code:", FontSize = 12, FontAttributes = FontAttributes.Bold }, 0, 4);
            grid.Add(new Label { Text = client.postal_code, FontSize = 12 }, 1, 4);

            grid.Add(new Label { Text = "Phone number:", FontSize = 12, FontAttributes = FontAttributes.Bold }, 0, 5);
            grid.Add(new Label { Text = client.phone, FontSize = 12 }, 1, 5);

            grid.Add(new Label { Text = "Email:", FontSize = 12, FontAttributes = FontAttributes.Bold }, 0, 6);
            grid.Add(new Label { Text = client.email, FontSize = 12 }, 1, 6);

            grid.Add(new Label { Text = "Reccurence:", FontSize = 12, FontAttributes = FontAttributes.Bold }, 0, 7);
            grid.Add(new Label { Text = client.recurrence, FontSize = 12 }, 1, 7);

            grid.Add(new Label { Text = "Numbers of containers:", FontSize = 12, FontAttributes = FontAttributes.Bold }, 0, 8);
            grid.Add(new Label { Text = client.container_count.ToString(), FontSize = 12 }, 1, 8);

            grid.Add(new Label { Text = "Start date:", FontSize = 12, FontAttributes = FontAttributes.Bold }, 0, 9);
            grid.Add(new Label { Text = client.start_date == DateTime.MinValue ? "Not specified" : client.start_date.ToString("yyyy-MM-dd"), FontSize = 12 }, 1, 9);

            frame.Content = grid;

            // Добавляем обработчик нажатия
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += async (s, e) => await OnClientTapped(client);
            frame.GestureRecognizers.Add(tapGesture);

            ClientsStackLayout.Children.Add(frame);
        }
    }
    private void DisplayNoDataMessage()
    {
        ClientsStackLayout.Children.Clear();

        var emptyLabel = new Label
        {
            Text = "No data. Add clients on the main page.",
            FontSize = 14,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        ClientsStackLayout.Children.Add(emptyLabel);
    }

    private async Task OnClientTapped(ClientViewModel client)
    {
        await Navigation.PushAsync(new EditClientPage(client, this));
    }

    public async Task RefreshDataAsync()
    {
        await LoadDataAsync();
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
}
