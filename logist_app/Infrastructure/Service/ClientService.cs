using logist_app.Core.Entities;
using logist_app.Core.Interfaces;
using logist_app.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

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
            var http = httpFactory.CreateClient("Api");
            var clients = await http.GetFromJsonAsync<ObservableCollection<Client>>($"{api.ClientsEndpoint}");
            return clients ?? new ObservableCollection<Client>();
        }

    }
}
