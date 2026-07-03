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
            .ConfigureServices((hostContext, services) =>
            {

                services.AddSingleton<Func<Type, ViewModelBase>>(provider =>
                    type => (ViewModelBase)provider.GetRequiredService(type));
                services.AddSingleton<INavigationService, NavigationService>();

                // 🖥️ 2. Ventana principal
                services.AddSingleton<MainWindowViewModel>();
                
                // 📂 CONFIGURACIONES (Lee automáticamente appsettings.json si existe)
                // services.Configure<RestauranteConfig>(hostContext.Configuration.GetSection("RestauranteConfig"));

                // 🛠️ SERVICIOS (Singletons para conexiones e infraestructura global)
                // services.AddSingleton<ISignalRService, SignalRService>();
                // services.AddSingleton<IPrinterService, ThermalPrinterService>(); 

                // 🖥️ VIEWMODELS PRINCIPALES (Singletons porque controlan estados globales)
                // services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<LoginViewModel>();

                // 📄 VIEWMODELS DE PANTALLAS (Transients para que se limpien al entrar/salir)
                services.AddTransient<TablesViewModel>();
                services.AddTransient<NewOrderViewModel>();
                // services.AddTransient<OrderViewModel>();
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
