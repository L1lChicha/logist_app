namespace logist_app;
using logist_app.ViewModels;
using logist_app.Factories;
using logist_app.Models;

public partial class EditClientPage : ContentPage
{

    private readonly EditClientViewModel _vm;

    public EditClientPage(EditClientViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    public void Initialize(Client client)
    {
        _vm.Initialize(client);
    }
}
