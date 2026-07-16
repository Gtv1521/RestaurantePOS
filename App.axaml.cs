using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Interfaces;
using MiComanderaApp.ViewModels;
using MiComanderaApp.Views;
using Microsoft.Extensions.DependencyInjection;

namespace MiComanderaApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        LiveCharts.Configure(config => config.AddSkiaSharp());
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindowViewModel = Program.AppHost?
                .Services
                .GetRequiredService<MainWindowViewModel>();

            var mainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };

            // Registrar la instancia en DI
            var windowProvider = Program.AppHost?
                .Services
                .GetRequiredService<IWindowProvider>();

            windowProvider!.MainWindow = mainWindow;


            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}