using CommunityToolkit.Mvvm.ComponentModel;
using logist_app.Core.Entities;

namespace logist_app.ViewModels
{
    public partial class SelectableClient : ObservableObject
    {
        public Client Client { get; }

        [ObservableProperty]
        private bool isSelected;

        public SelectableClient(Client client)
        {
            Client = client;
        }

        // Прокидываем свойства Client для удобства биндинга
        public int Id => Client.Id;
        public string Name => Client.Name;
        public string Address => Client.Address;
        public string City => Client.City;
        public string PostalCode => Client.PostalCode;
        public string Phone => Client.Phone;
        public string Email => Client.Email;
        public string Recurrence => Client.Recurrence;
        public int ContainerCount => Client.ContainerCount;
        public DateTime StartDate => Client.StartDate;
        public string Coordinates => Client.Coordinates;
        public string? LoadingType => Client.LoadingType;
        public double Volume => Client.Volume;
        public double Lat => Client.Lat;
        public double Lon => Client.Lon;
    }
}

