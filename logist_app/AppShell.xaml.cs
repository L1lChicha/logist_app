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
            Routing.RegisterRoute(nameof(AppSettingsView), typeof(AppSettingsView));
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Current.GoToAsync(nameof(AppSettingsView));
        }

        private async void OnNotificationsClicked(object sender, EventArgs e)
        {
            await Current.GoToAsync(nameof(NotificationsView));
        }
    }
}
