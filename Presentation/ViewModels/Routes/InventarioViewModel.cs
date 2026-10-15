using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.ViewModels.Components.Admin;

namespace MiComanderaApp.ViewModels.Routes;

public partial class InventarioViewModel : ViewModelBase
{
    private readonly IViewModelFactory _factory;
    private readonly INavigationService _navigation;
    private readonly ISession<SessionModel> _userSesion;

    [ObservableProperty] public SessionModel? _user;
    [ObservableProperty] private object? _vistaActual;
    [ObservableProperty] private string _currentView = "Inventario";

    public bool IsInventario => CurrentView == "Inventario";
    public bool IsReportes => CurrentView == "Reportes";

    public InventarioViewModel(
        IViewModelFactory factory,
        INavigationService navigation,
        ISession<SessionModel> userSesion)
    {
        _factory = factory;
        _navigation = navigation;
        _userSesion = userSesion;
        InitializeComponent();
    }

    public void InitializeComponent()
    {
        User = _userSesion.Model;
        CurrentView = "Inventario";
        VistaActual = _factory.Create<InventarioComponentViewModel>();
    }

    partial void OnCurrentViewChanged(string value)
    {
        OnPropertyChanged(nameof(IsInventario));
        OnPropertyChanged(nameof(IsReportes));
    }

    [RelayCommand]
    private void ChangeView(string vista)
    {
        CurrentView = vista;

        VistaActual = vista switch
        {
            "Inventario" => _factory.Create<InventarioComponentViewModel>(),
            "Reportes" => _factory.Create<ReportesInventarioViewModel>(),
            _ => _factory.Create<InventarioComponentViewModel>(),
        };
    }

    [RelayCommand]
    private void Salir()
    {
        _userSesion.LogoutAsync();
        _navigation.NavigateTo<LoginViewModel>();
    }
}
