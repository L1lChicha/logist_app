using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace logist_app
{
    public partial class App : Application
    {

        public static IServiceProvider Services { get; private set; }
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            Services = serviceProvider;

        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }


    }
}