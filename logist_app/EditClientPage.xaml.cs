using Microsoft.Maui.Controls;
using Npgsql;
using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace logist_app;

public partial class EditClientPage : ContentPage
{
    private readonly ClientViewModel _client;
    private readonly DataViewPage _parentPage;
    private readonly Entry _nameEntry;
    private readonly Entry _addressEntry;
    private readonly Entry _cityEntry;
    private readonly Entry _postalCodeEntry;
    private readonly Entry _phoneEntry;
    private readonly Entry _emailEntry;
    private readonly Entry _recurrenceEntry;
    private readonly Entry _containerCountEntry;
    private readonly DatePicker _startDatePicker;
    private const string ApiUrl = "https://localhost:32769/api/Clients";

    public EditClientPage(ClientViewModel client, DataViewPage parentPage)
    {
        _client = client;
        _parentPage = parentPage;

        Title = "Edit client";



        var stackLayout = new StackLayout
        {
            Padding = new Thickness(10),
            Spacing = 10
        };

        stackLayout.Children.Add(new Label { Text = "Name:", FontSize = 14 });
        _nameEntry = new Entry { Text = client.name, FontSize = 14 };
        stackLayout.Children.Add(_nameEntry);

        stackLayout.Children.Add(new Label { Text = "Address:", FontSize = 14 });
        _addressEntry = new Entry { Text = client.address, FontSize = 14 };
        stackLayout.Children.Add(_addressEntry);

        stackLayout.Children.Add(new Label { Text = "City:", FontSize = 14 });
        _cityEntry = new Entry { Text = client.city, FontSize = 14 };
        stackLayout.Children.Add(_cityEntry);

        stackLayout.Children.Add(new Label { Text = "Post code:", FontSize = 14 });
        _postalCodeEntry = new Entry { Text = client.postal_code, FontSize = 14 };
        stackLayout.Children.Add(_postalCodeEntry);

        stackLayout.Children.Add(new Label { Text = "Phone number:", FontSize = 14 });
        _phoneEntry = new Entry { Text = client.phone, FontSize = 14 };
        stackLayout.Children.Add(_phoneEntry);

        stackLayout.Children.Add(new Label { Text = "Email:", FontSize = 14 });
        _emailEntry = new Entry { Text = client.email, FontSize = 14 };
        stackLayout.Children.Add(_emailEntry);

        stackLayout.Children.Add(new Label { Text = "Reccurrence:", FontSize = 14 });
        _recurrenceEntry = new Entry { Text = client.recurrence, FontSize = 14 };
        stackLayout.Children.Add(_recurrenceEntry);

        stackLayout.Children.Add(new Label { Text = "Number of containers:", FontSize = 14 });
        _containerCountEntry = new Entry { Text = client.container_count.ToString(), FontSize = 14, Keyboard = Keyboard.Numeric };
        stackLayout.Children.Add(_containerCountEntry);

        stackLayout.Children.Add(new Label { Text = "Start date:", FontSize = 14 });
        _startDatePicker = new DatePicker
        {
            Date = client.start_date == DateTime.MinValue ? DateTime.Today : client.start_date,
            FontSize = 14
        };
        stackLayout.Children.Add(_startDatePicker);

        var saveButton = new Button
        {
            Text = "Save",
            FontSize = 14,
            HeightRequest = 40,
            WidthRequest = 100
        };
        saveButton.Clicked += async (s, e) => await SaveChangesAsync();
        stackLayout.Children.Add(saveButton);

        var cancelButton = new Button
        {
            Text = "Cancel",
            FontSize = 14,
            HeightRequest = 40,
            WidthRequest = 100
        };
        cancelButton.Clicked += async (s, e) => await Navigation.PopAsync();
        stackLayout.Children.Add(cancelButton);

        var deleteClientButton = new Button
        {
            Text = "Delete client",
            FontSize = 14,
            HeightRequest = 40,
            WidthRequest = 100
        };

        deleteClientButton.Clicked += async (s, e) =>
        {
            var confirm = await DisplayAlert("Confirm", $"Delete client {client.name}?", "Yes", "No");
            if (!confirm)
                return;

            using var httpClient = new HttpClient();
            var response = await httpClient.DeleteAsync($"{ApiUrl}/{client.id}");

            if (response.IsSuccessStatusCode)
            {
                //Clients.Remove(client); // Удалить из ObservableCollection
                //await DisplayAlert("Success", "Client deleted.", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Failed to delete client.", "OK");
            }
        };

        stackLayout.Children.Add(deleteClientButton);


        Content = new ScrollView { Content = stackLayout };
    }

    private async Task<bool> UpdateClientAsync(ClientViewModel client)
    {
        var ApiUrl = $"https://localhost:32769/api/Clients/{client.id}";

        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PutAsJsonAsync(ApiUrl, client);

            var json = JsonSerializer.Serialize(client);


            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Client updated successfully.");
                return true;
            }
            else
            {
                await DisplayAlert("Error", $"Failed to update client: {response.StatusCode}", "OK");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during update: {ex.Message}");
            return false;
        }

    }

    private async Task SaveChangesAsync()
    {
        _client.name = _nameEntry.Text;
        _client.address = _addressEntry.Text;
        _client.city = _cityEntry.Text;
        _client.postal_code = _postalCodeEntry.Text;
        _client.phone = _phoneEntry.Text;
        _client.email = _emailEntry.Text;
        _client.recurrence = _recurrenceEntry.Text;
        _client.container_count = int.Parse(_containerCountEntry.Text);
        _client.start_date = _startDatePicker.Date;

        // Отправить обновления на сервер
        var success = await UpdateClientAsync(_client);
        if (success)
        {
            await DisplayAlert("Success", "Client updated successfully.", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error сохранение", "Failed to update client.", "OK");
        }
    }


   
}