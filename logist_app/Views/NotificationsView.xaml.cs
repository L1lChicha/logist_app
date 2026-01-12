using logist_app.ViewModels;

namespace logist_app.Views;


    public partial class NotificationsView : ContentPage
    {
        public NotificationsView(NotificationsViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
