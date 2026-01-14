using logist_app.Core.Entities;
using logist_app.Core.Interfaces;
using logist_app.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace logist_app.Infrastructure.Service
{
    public class ClientService : IClientService 
    {
        private readonly IHttpClientFactory httpFactory;
        private readonly ApiSettings api;

        public ClientService(IHttpClientFactory _httpClientFactory, ApiSettings _apiSettings )
        {
            httpFactory = _httpClientFactory;
            api = _apiSettings;    
        }

        public async Task<ObservableCollection<Client>> GetClientsAsync()
        {
            try
            {
                var http = httpFactory.CreateClient("Api");

                // --- ДОБАВЛЯЕМ ТОКЕН ---
                var token = await SecureStorage.Default.GetAsync("auth_token");
                if (!string.IsNullOrEmpty(token))
                {
                    http.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }
                // -----------------------

                // Пробуем получить данные с сервера
                var clients = await http.GetFromJsonAsync<ObservableCollection<Client>>(api.ClientsEndpoint);

                return clients ?? new ObservableCollection<Client>();
            }
            catch (HttpRequestException ex)
            {
                // Ошибки сети, DNS, сервер недоступен, таймаут
                Debug.WriteLine($"[GetClientsAsync] Network error: {ex.Message}");
            }
            catch (NotSupportedException ex)
            {
                // Невалидный контент (сервер вернул не JSON)
                Debug.WriteLine($"[GetClientsAsync] Invalid content type: {ex.Message}");
            }
            catch (JsonException ex)
            {
                // Невалидный JSON
                Debug.WriteLine($"[GetClientsAsync] JSON parse error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Любые другие ошибки
                Debug.WriteLine($"[GetClientsAsync] Unknown error: {ex.Message}");
            }

            // Возвращаем пустую коллекцию при любой ошибке
            return new ObservableCollection<Client>();
        }

    }
}
