using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using logist_app.Models;

namespace logist_app.ViewModels
{
    public partial class ClientViewModel : ObservableObject
    {
        public Client Model { get; }

        public ClientViewModel(Client model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

       
        public int Id
        {
            get => Model.Id;
            set
            {
                if (Model.Id == value) return;
                Model.Id = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => Model.Name;
            set
            {
                if (Model.Name == value) return;
                Model.Name = value;
                OnPropertyChanged();
            }
        }

        public string Address
        {
            get => Model.Address;
            set
            {
                if (Model.Address == value) return;
                Model.Address = value;
                OnPropertyChanged();
            }
        }

        public string Phone
        {
            get => Model.Phone;
            set
            {
                if (Model.Phone == value) return;
                Model.Phone = value;
                OnPropertyChanged();
            }
        }

        public DateTime? StartDate
        {
            get => Model.StartDate;
            set
            {
                if (Model.StartDate == value) return;
                Model.StartDate = value ?? default;
                OnPropertyChanged();
            }
        }

        public string City
        {
            get => Model.City;
            set
            {
                if(Model.City == value) return;
                Model.City = value;
                OnPropertyChanged();
            }
        }

        public string PostalCode
        {
            get => Model.PostalCode;
            set
            {
                if(Model.PostalCode == value)  return;
                Model.PostalCode = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => Model.Email;
            set
            {
                if (Model.Email == value) return;
                Model.Email = value;
                OnPropertyChanged();
            }
        }

        public string Recurrence
        {
            get => Model.Recurrence;
            set
            {
                if (Model.Recurrence == value) return;
                Model.Recurrence = value;
                OnPropertyChanged();
            }
        }

        public int ContainerCount
        {
            get => Model.ContainerCount;
            set
            {
                if (Model.ContainerCount == value) return;
                Model.ContainerCount = value;
                OnPropertyChanged();
            }
        }

        public string Coordinates
        {
            get => Model.Coordinates;
            set
            {
                if (Model.Coordinates == value) return;
                Model.Coordinates = value;
                OnPropertyChanged();
            }
        }

        public double Lat
        {
            get => Model.Lat;
            set
            {
                if (Model.Lat == value) return;
                Model.Lat = value;
                OnPropertyChanged();
            }
        }

        public double Lon
        {
            get => Model.Lon;
            set
            {
                if (Model.Lon == value) return;
                Model.Lon = value;
                OnPropertyChanged();
            }
        }


        [ObservableProperty]
        private bool isSelected;
    }
    
}
