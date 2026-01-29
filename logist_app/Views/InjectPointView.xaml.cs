using logist_app.ViewModels;

namespace logist_app.Views;

public partial class InjectPointView : ContentPage
{
	private readonly InjectPointViewModel _injectPointViewModel;
	public InjectPointView(InjectPointViewModel injectPointViewModel)
	{
		_injectPointViewModel = injectPointViewModel;
        InitializeComponent();
        BindingContext = _injectPointViewModel;

    }
}