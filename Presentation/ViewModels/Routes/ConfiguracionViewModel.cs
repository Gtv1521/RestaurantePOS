using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.ViewModels.Components.Configuracion;

namespace MiComanderaApp.ViewModels.Routes;

public partial class ConfiguracionViewModel : ViewModelBase
{
    private readonly IViewModelFactory _factory;
    private readonly INavigationService _navigation;
    private readonly ISession<SessionModel> _userSesion;

    [ObservableProperty] public SessionModel? _user;
    [ObservableProperty] private object? _vistaActual;
    [ObservableProperty] private string _currentView = "General";

    public bool IsGeneral => CurrentView == "General";
    public bool IsImpresoras => CurrentView == "Impresoras";

    public ConfiguracionViewModel(
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
        CurrentView = "General";
        VistaActual = _factory.Create<GeneralSettingsComponentViewModel>();
    }

    partial void OnCurrentViewChanged(string value)
    {
        OnPropertyChanged(nameof(IsGeneral));
        OnPropertyChanged(nameof(IsImpresoras));
    }

    [RelayCommand]
    private void ChangeView(string vista)
    {
        CurrentView = vista;

        VistaActual = vista switch
        {
            "General" => _factory.Create<GeneralSettingsComponentViewModel>(),
            "Impresoras" => _factory.Create<PrintersComponentViewModel>(),
            _ => _factory.Create<GeneralSettingsComponentViewModel>(),
        };
    }

    [RelayCommand]
    private void Volver()
    {
        var vm = _factory.Create<AdminDashboardViewModel>();
        _navigation.NavigateTo(vm);
    }

    [RelayCommand]
    private void Salir()
    {
        _userSesion.LogoutAsync();
        _navigation.NavigateTo<LoginViewModel>();
    }
}
