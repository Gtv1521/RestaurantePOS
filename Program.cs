using Avalonia;
using System;
using Avalonia.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MiComanderaApp.ViewModels;
using MiComanderaApp.ViewModels.Orders;
using Avalonia.ReactiveUI;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Services.Routing;
using MiComanderaApp.Models;
using MiComanderaApp.Services.Conections;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MiComanderaApp;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    public static IHost? AppHost { get; private set; }

    [STAThread]
    public static void Main(string[] args)
    {
        // 1. Inicializamos el Host de .NET y registramos todos los servicios
        AppHost = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                // Aseguramos que lea el archivo appsettings.json en la raíz
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<ApiSettings>(hostContext.Configuration.GetSection("ApiSettings"));
                services.AddSingleton<Func<Type, ViewModelBase>>(provider =>
                    type => (ViewModelBase)provider.GetRequiredService(type));
                services.AddSingleton<INavigationService, NavigationService>();

                // 🖥️ 2. Ventana principal
                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<LoginViewModel>();

                services.AddTransient<TablesViewModel>();
                services.AddTransient<NewOrderViewModel>();

                // 🖥️ 3. Servicios
                services.AddSingleton<HttpClient>();
                services.AddScoped<ISession<SessionModel>, SessionService>();

            })
            .Build();

        // 2. Encendemos el Host en segundo plano
        AppHost.Start();

        // 3. Arrancamos Avalonia normalmente como lo tenías originalmente
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .LogToTrace();
}
