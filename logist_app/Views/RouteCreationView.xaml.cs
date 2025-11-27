using CommunityToolkit.Mvvm.Messaging;
using logist_app.Models;
using logist_app.ViewModels;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;

namespace logist_app.Views;

public partial class RouteCreationView : ContentPage
{
    private readonly RouteCreationViewModel _viewModel;
    public RouteCreationView(RouteCreationViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        

    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadClientsAsync();
    }




   // public async Task RefreshClients() => await _viewModel.LoadClientsAsync();

    //private async void OnRefreshClicked(object sender, EventArgs e) =>
     //   await _viewModel.LoadClientsAsync();


    

   
}  
