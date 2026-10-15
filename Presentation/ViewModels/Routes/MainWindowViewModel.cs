using CommunityToolkit.Mvvm.ComponentModel;
using MiComanderaApp.Core.Infrastructure.SignalR;
using MiComanderaApp.Core.Infrastructure.SignalR.Events;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Presentation.Services;
using MiComanderaApp.ViewModels.Orders;
using Microsoft.Extensions.DependencyInjection;

namespace MiComanderaApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public INavigationService Navigation { get; }
    public LoadingState Loading { get; }
    private readonly SignalRService _signalR;
    private readonly SignalREventRegistry _registrar;

    public MainWindowViewModel(INavigationService navigationService, SignalRService signalR, SignalREventRegistry registrar, LoadingState loading)
    {
        Navigation = navigationService;
        Loading = loading;
        _registrar = registrar;
        _signalR = signalR;
        Navigation.NavigateTo<LoginViewModel>(); // se define el primer ViewModel que se mostrará al iniciar la aplicación
        Initialize();
    }

    private async void Initialize()
    {

        await _signalR.ConnectAsync();
        _registrar.RegisterAll(_signalR);

    }
}
