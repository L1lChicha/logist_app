using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace logist_app;

public class DataViewModel : INotifyPropertyChanged
{
    public ObservableCollection<ClientViewModel> Clients { get; } = new();

    private const string ApiUrl = "https://localhost:32769/api/Clients";

    public DataViewModel()
    {
        LoadDataAsync();
    }

    public async Task LoadDataAsync()
    {
        try
        {
            Clients.Clear();
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(ApiUrl);

            if (response.IsSuccessStatusCode)
            {
                var clients = await response.Content.ReadFromJsonAsync<ObservableCollection<ClientViewModel>>();
                if (clients != null)
                {
                    foreach (var client in clients)
                        Clients.Add(client);
                }
            }
        }
        catch (Exception ex)
        {
            // Обработка ошибок
            Console.WriteLine($"Ошибка загрузки данных: {ex.Message}");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
