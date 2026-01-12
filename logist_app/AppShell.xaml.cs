using logist_app.Views;
using Plugin.LocalNotification;
namespace logist_app
{

    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(NotificationsView), typeof(NotificationsView));
        }


        private async void OnNotificationsClicked(object sender, EventArgs e)
        {
            await Current.GoToAsync(nameof(NotificationsView));
        }
    }
}
