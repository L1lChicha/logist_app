using CommunityToolkit.Mvvm.ComponentModel;

using logist_app.Models;
using logist_app.ViewModels;
using logist_app.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace logist_app;

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
        await _viewModel.LoadDataAsync();
    }
    //  public async Task RefreshClients() => await _viewModel.LoadDataAsync();

    private async void OnRefreshClicked(object sender, EventArgs e) =>
        await _viewModel.LoadDataAsync();
   
}
