using logist_app.Models;
using logist_app.ViewModels;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace logist_app.Factories
{
    public interface IEditClientVmFactory
    {
        EditClientViewModel Create(ClientViewModel client, INavigation nav, Func<Task> refresh);
    }

    public class EditClientVmFactory : IEditClientVmFactory
    {
        private readonly ApiSettings _api;
        private readonly IHttpClientFactory _http;

        public EditClientVmFactory(ApiSettings api, IHttpClientFactory http)
        {
            _api = api;
            _http = http;
        }

        public EditClientViewModel Create(ClientViewModel client, INavigation nav, Func<Task> refresh)
        {
            return new EditClientViewModel(client, nav, refresh, _api, _http);
        }
    }
}