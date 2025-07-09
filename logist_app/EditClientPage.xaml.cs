namespace logist_app;

public partial class EditClientPage : ContentPage
{
    public EditClientPage(ClientViewModel client, Func<Task> refreshCallback)
    {
        InitializeComponent();
        BindingContext = new EditClientViewModel(client, Navigation, refreshCallback);
    }
}
