using logist_app.ViewModels;

namespace logist_app.Views;

public partial class ShiftsPageView : ContentPage
{
	private readonly ShiftsViewModel _viewModel;

	public ShiftsPageView(ShiftsViewModel viewModel)
	{
		
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
	}

    private bool _hasLoaded = false;
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Загружаем данные только если они еще не были загружены
        if (!_hasLoaded)
        {
            await _viewModel.LoadShiftsAsync();

            // Отмечаем, что загрузка успешно выполнена
            _hasLoaded = true;
        }
    }
}