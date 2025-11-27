using logist_app.Core.Entities;
using System.Collections.ObjectModel;


namespace logist_app.Core.Interfaces
{
    public interface IClientService
    {
        Task<ObservableCollection<Client>> GetClientsAsync();
    }
}
