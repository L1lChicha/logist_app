namespace logist_app;
using logist_app.ViewModels;
using logist_app.Factories;
using logist_app.ViewModels;

public partial class EditClientPage : ContentPage
{
    public EditClientPage(ClientViewModel client, Func<Task> refresh, IEditClientVmFactory factory)
    {
        InitializeComponent();
        BindingContext = factory.Create(client, Navigation, refresh);
    }
}
