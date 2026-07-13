using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.ViewModels.Components.Admin;
using SkiaSharp;

namespace MiComanderaApp.ViewModels.Routes;

public partial class AdminDashboardViewModel : ViewModelBase
{
    private readonly IViewModelFactory _factory;
    private readonly INavigationService _navigation;
    private readonly ISession<SessionModel> _userSesion;


    [ObservableProperty] public SessionModel? _user;
    [ObservableProperty] private object? _vistaActual;
    [ObservableProperty] private string currentView = "Inicio";

    public bool IsMenu => CurrentView == "Menu";
    public bool IsInicio => CurrentView == "Inicio";
    public bool IsMesas => CurrentView == "Mesas";
    public bool IsClientes => CurrentView == "Clientes";
    public bool IsConfiguracion => CurrentView == "Configuracion";

    public AdminDashboardViewModel(IViewModelFactory factory, INavigationService navigation, ISession<SessionModel> userSesion)
    {
        _factory = factory;
        _navigation = navigation;
        _userSesion = userSesion;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        User = _userSesion.Model!;
        VistaActual = _factory.Create<EstadisticasComponentViewModel>();
    }
    // public void Initialize()
    // {

    // }

    partial void OnCurrentViewChanged(string value)
    {
        OnPropertyChanged(nameof(IsMenu));
        OnPropertyChanged(nameof(IsInicio));
        OnPropertyChanged(nameof(IsMesas));
        OnPropertyChanged(nameof(IsClientes));
        OnPropertyChanged(nameof(IsConfiguracion));
    }

    [RelayCommand]
    private void Salir()
    {
        _navigation.NavigateTo<LoginViewModel>();
    }

    [RelayCommand]
    private void ChangeView(string vista)
    {
        CurrentView = vista;

        VistaActual = vista switch
        {
            "Inicio" => _factory.Create<EstadisticasComponentViewModel>(),
            "Menu" => _factory.Create<MenuComponentViewModel>(),
            _ => _factory.Create<EstadisticasComponentViewModel>(),
        };
    }


    [RelayCommand]
    private void IrAMesas()
    {
        var vm = _factory.Create<TablesViewModel>();
        _navigation.NavigateTo(vm);
    }
};