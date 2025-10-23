using System.Reflection;
using Microsoft.Extensions.Configuration;
using CommunityToolkit.Maui;
using Microsoft.Maui.Maps;
using Microsoft.Extensions.Logging;
using logist_app.Models;
using logist_app.ViewModels;
using logist_app.Views;
using Microsoft.Extensions.DependencyInjection;
using logist_app.Factories;

namespace logist_app
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>() // это вызовет App(IServiceProvider)
                .UseMauiCommunityToolkit()
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
            builder.Services.AddSingleton<IEditClientVmFactory, EditClientVmFactory>();


            // ✅ Регистрируем типизированные настройки
            var apiSettings = new ApiSettings();
            config.GetSection("ApiSettings").Bind(apiSettings);

            builder.Services.AddSingleton(apiSettings);
            builder.Services.AddHttpClient("Api", http =>
            {
                http.BaseAddress = new Uri(apiSettings.BaseUrl);
            });

            // ✅ Регистрируем ViewModels
            builder.Services.AddSingleton<DataViewModel>();
            builder.Services.AddTransient<DataViewPage>();
            builder.Services.AddTransient<RouteCreationPage>();
            builder.Services.AddSingleton<IEditClientVmFactory, EditClientVmFactory>();
            builder.Services.AddSingleton<RoutesListViewModel>();
            builder.Services.AddTransient<ViewRoutesPage>();
            builder.Services.AddTransient<DataViewModel>();
            builder.Services.AddTransient<DataViewPage>();
            builder.Services.AddTransient<RouteCreationPage>();
            builder.Services.AddTransient<DriverManagerView>();
            builder.Services.AddTransient<AddNewDriverPage>();
            builder.Services.AddSingleton<DriversViewModel>();
            builder.Services.AddTransient<DriversDataView>();

            //  builder.Services.AddTransient<AddClientViewModel>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // создаём App с контейнером
            return app;
        }
    }
}
