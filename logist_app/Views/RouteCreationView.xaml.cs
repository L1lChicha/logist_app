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

    //private void OnClientsSelected(object sender, SelectionChangedEventArgs e)
    //{
    //    foreach (var added in e.CurrentSelection.OfType<ClientViewModel>())
    //        added.IsSelected = true;

    //    foreach (var removed in e.PreviousSelection.OfType<ClientViewModel>()
    //                                               .Except(e.CurrentSelection.OfType<ClientViewModel>()))
    //        removed.IsSelected = false;

    //    var selected = e.CurrentSelection.OfType<ClientViewModel>().ToList();
    //    selectedClientIds = selected.Select(c => c.Id).ToList();
    //    createRouteButton.IsEnabled = selected.Count > 1;
    //    if (createRouteButton.IsEnabled)
    //    {
    //        createRouteButton.BackgroundColor = Color.FromArgb("#62b375");
    //    }
    //    else
    //    {
    //        createRouteButton.BackgroundColor = Color.FromArgb("#404040");
    //    }
        
    //}

    

   
}  
