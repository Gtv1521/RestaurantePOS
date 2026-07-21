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
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using MiComanderaApp.ViewModels.Mesas;
using MiComanderaApp.Services.Factorys;
using MiComanderaApp.ViewModels.Components;
using MiComanderaApp.Views.Routes;
using MiComanderaApp.ViewModels.Routes;
using MiComanderaApp.ViewModels.Components.Admin;
using MiComanderaApp.ViewModels.Components.Products;
using MiComanderaApp.Infrastructure.Api;
using MiComanderaApp.Core.Infrastructure.SignalR;
using LiveChartsCore.Drawing.Segments;
using MiComanderaApp.Core.Infrastructure.SignalR.Events;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Application.UseCases.Session;
using MiComanderaApp.Core.Application.UseCases.Catalogo;
using MiComanderaApp.Core.Infrastructure.Api;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Models;
using System.Net;
using Microsoft.Extensions.Options;
using MiComanderaApp.Presentation.States;
using MiComanderaApp.Views;
using MiComanderaApp.Services;
using MiComanderaApp.Core.Application.UseCases.Product;
using MiComanderaApp.ViewModels.Dialogs.Modals;
using MiComanderaApp.Views.Dialogs.Modals;
using MiComanderaApp.Presentation.Services;
using RestaurantePOS.Core.Application.UseCases.Ingrediente;
using RestaurantePOS.Core.Application.Request;
using MiComanderaApp.Presentation.Views.Dialogs.Modals;

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
                services.AddSingleton<IViewModelFactory, ViewModelFactory>();
                services.Configure<ApiSettings>(hostContext.Configuration.GetSection("ApiSettings"));
                services.AddSingleton<Func<Type, ViewModelBase>>(provider =>
                    type => (ViewModelBase)provider.GetRequiredService(type));

                services.AddSingleton(sp =>
                {
                    var cookies = new CookieContainer();
                    var handler = new HttpClientHandler
                    {
                        CookieContainer = cookies,
                        UseCookies = true
                    };

                    var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;

                    return new HttpClient(handler)
                    {
                        BaseAddress = new Uri(settings.BaseUrl)
                    };
                });

                services.AddSingleton<IWindowProvider, WindowProvider>();
                services.AddSingleton<IDialogService, DialogService>();
                services.AddSingleton<INavigationService, NavigationService>();

                // 🖥️ 2. Ventana principal
                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<LoginViewModel>();
                services.AddSingleton<AdminDashboardViewModel>();

                services.AddTransient<TablesViewModel>();
                services.AddTransient<TableViewModel>();
                services.AddTransient<NewOrderViewModel>();
                services.AddTransient<DataTableViewModel>();
                services.AddTransient<ActiveTablesComponentViewModel>();
                services.AddTransient<AllTablesComponentViewModel>();
                services.AddTransient<EstadisticasComponentViewModel>();
                services.AddTransient<MenuComponentViewModel>();
                services.AddTransient<ProductViewModel>();
                services.AddSingleton<CantidadPaxViewModel>();
                services.AddTransient<InventarioComponentViewModel>();

                // global states
                services.AddScoped<TableState>();

                // 🖥️ 3. Servicios
                services.AddSingleton<HttpClient>();
                services.AddScoped<ISession<SessionModel>, SessionService>();
                services.AddScoped<IMultipleCrud<CatalogoModel, CatalogoRequest>, CatalogoRepository>();
                services.AddScoped<IMultipleCrud<ProductoModel, ProductoRequest>, ProductoRepository>();
                services.AddScoped<IGetList<ProductoModel>, ProductoRepository>();
                services.AddScoped<ISingleCrud<TableModel, TableRequest>, TablesRepository>();
                services.AddScoped<IMultipleCrud<IngredienteModel, IngredienteRequest>, InventarioRepository>();

                // usescases 
                services.AddScoped<GetSessionSave>();
                services.AddScoped<GetAllCatalogoUseCase>();
                services.AddScoped<GetCatalogoXIdProdUseCase>();
                services.AddScoped<GetAllProductUseCase>();
                services.AddScoped<CreateIngredienteUseCase>();
                services.AddScoped<UpdateIngredienteUseCase>();
                services.AddScoped<DeleteIngredienteUseCase>();
                services.AddScoped<GetAllIngredientesUseCase>();

                // signalR
                services.AddSingleton<SignalRService>();
                services.AddSingleton<SignalREventRegistry>();
                services.AddScoped<ISignalREventHandler, OrderEvents>();
                services.AddScoped<ISignalREventHandler, TablesEvents>();


                services.AddSingleton<MainWindow>();
                services.AddTransient<CreateProductViewModel>();
                services.AddTransient<CreateProduct>();
                services.AddTransient<IngredienteDialogViewModel>();
                services.AddTransient<IngredienteDialog>();
                services.AddSingleton<IDialogService, DialogService>();
            })
            .Build();

        // 2. Encendemos el Host en segundo plano
        AppHost.Start();

        // 3. Arrancamos Avalonia normalmente como lo tenías originalmente
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        IconProvider.Current.Register<FontAwesomeIconProvider>();

        return AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .LogToTrace();
    }
}
