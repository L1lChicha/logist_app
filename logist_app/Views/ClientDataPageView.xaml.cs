using CommunityToolkit.Mvvm.ComponentModel;

using logist_app.Models;
using logist_app.ViewModels;
using logist_app.Infrastructure.Service;
using logist_app.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace logist_app.Views;

public partial class ClientDataPageView: ContentPage
{
    private readonly ClientDataViewModel _viewModel;

    public ClientDataPageView(ClientDataViewModel viewModel)
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
    

   
}
