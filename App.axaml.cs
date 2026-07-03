using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
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
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // 1. Le pedimos al contenedor global que fabrique el MainWindowViewModel.
            // Al hacerlo, .NET inyectará automáticamente el NavigationService en su constructor,
            // y este a su vez ejecutará el Navigation.NavigateTo<LoginViewModel>().
            var mainWindowViewModel = Program.AppHost?.Services.GetRequiredService<MainWindowViewModel>();

            // 2. Creamos la ventana principal del sistema operativo
            desktop.MainWindow = new MainWindow
            {
                // 3. Le asignamos el ViewModel resuelto como su contexto de datos (DataContext)
                DataContext = mainWindowViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}