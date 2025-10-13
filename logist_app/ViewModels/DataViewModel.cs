using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net.Http.Json;
using System.Threading.Tasks;
using logist_app.Models;

namespace logist_app.ViewModels;

public class DataViewModel : INotifyPropertyChanged
{
    public ObservableCollection<ClientViewModel> Clients { get; } = new();

    private readonly ApiSettings _apiSettings;
    private readonly IHttpClientFactory _httpClientFactory;

    public DataViewModel(ApiSettings apiSettings, IHttpClientFactory httpClientFactory)
    {
        _apiSettings = apiSettings;
        _httpClientFactory = httpClientFactory;

        // при желании можно не автозагружать и вызывать из страницы
        Task.Run(LoadDataAsync);
    }

    public async Task LoadDataAsync()
    {
        try
        {
            var http = _httpClientFactory.CreateClient("Api");
            var response = await http.GetAsync(_apiSettings.ClientsEndpoint);
            response.EnsureSuccessStatusCode();

            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var clients = await response.Content
                .ReadFromJsonAsync<List<ClientViewModel>>(options);

            Clients.Clear();
            if (clients is not null)
            {
                foreach (var c in clients)
                    Clients.Add(c);
            }

        }
        catch (Exception ex)
        {
            // лог — по вкусу
            System.Diagnostics.Debug.WriteLine($"LoadDataAsync error: {ex}");
        }
    
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
