using CommunityToolkit.Maui;
using logist_app.ViewModels;
using logist_app.Core.Interfaces;
using logist_app.Infrastructure.Service;
using logist_app.Models;
using logist_app.ViewModels;
using logist_app.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Maps;
using Plugin.LocalNotification;
using System.Reflection;
using Microsoft.UI.Xaml.Automation;

namespace logist_app
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>() 
                .UseMauiCommunityToolkit()
                .UseLocalNotification()
#if ANDROID || IOS || MACCATALYST
                .UseMauiMaps()
#endif

                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // ✅ Подключаем appsettings.json
            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("logist_app.appsettings.json");

            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            builder.Configuration.AddConfiguration(config);

            // ✅ Регистрируем IConfiguration
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
            //builder.Services.AddSingleton<IEditClientVmFactory, EditClientVmFactory>();


            // ✅ Регистрируем типизированные настройки
            var apiSettings = new ApiSettings();
            config.GetSection("ApiSettings").Bind(apiSettings);

            builder.Services.AddSingleton(apiSettings);
            builder.Services.AddHttpClient("Api", http =>
            {
                http.BaseAddress = new Uri(apiSettings.BaseUrl);
            });


            builder.Services.AddSingleton<SignalRService>();
            builder.Services.AddSingleton<IRouteService, RouteService>();
            // ✅ Регистрируем ViewModels

            builder.Services.AddSingleton<ClientDataViewModel>();
            builder.Services.AddSingleton<ClientDataPageView>();

            builder.Services.AddTransient<AddNewClientView>();

            builder.Services.AddSingleton<RouteCreationView>();
            builder.Services.AddSingleton<RouteCreationViewModel>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPageView>();

            builder.Services.AddSingleton<RoutesListViewModel>();
            builder.Services.AddSingleton<RoutesPageView>();

            builder.Services.AddSingleton<DriverManagerView>();

            builder.Services.AddSingleton<DriversDataView>();
            builder.Services.AddSingleton<DriversViewModel>();

            builder.Services.AddTransient<AddNewDriverPage>();
            

            builder.Services.AddTransient<EditClientViewModel>();
            builder.Services.AddTransient<EditClientPage>();


            builder.Services.AddSingleton<VehiclesDataView>();
            builder.Services.AddSingleton<VehiclesDataViewModel>();


            builder.Services.AddTransient<AddVehicleView>();
            builder.Services.AddTransient<AddVehicleViewModel>();

            builder.Services.AddTransient<AcceptRouteView>();
            builder.Services.AddTransient<AcceptRouteViewModel>();

            builder.Services.AddTransient<AddNewClientView>();
            builder.Services.AddTransient<AddNewClientViewModel>();



            builder.Services.AddTransient<IClientService, ClientService>();

            builder.Services.AddSingleton<NotificationsViewModel>();
            builder.Services.AddTransient<NotificationsView>();


#if DEBUG
            builder.Logging.AddDebug();
#endif
            var app = builder.Build();

            // создаём App с контейнером
            return app;
        }
    }
}
