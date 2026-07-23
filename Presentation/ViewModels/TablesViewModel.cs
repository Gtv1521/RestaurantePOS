﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.UseCases.Session;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.Request;
using MiComanderaApp.ViewModels.Components;
using MiComanderaApp.ViewModels.Orders;
using MiComanderaApp.ViewModels.Routes;
using MiComanderaApp.Views.Components;

namespace MiComanderaApp.ViewModels;

public partial class TablesViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IViewModelFactory _factory;
    private readonly GetSessionSave _userSesion;


    [ObservableProperty] private string _viewTitle = "Mesas Abiertas";
    [ObservableProperty] private string _toggleButtonText = "Todas las Mesas";
    [ObservableProperty] private bool _isShowingAllTables = true;
    [ObservableProperty] private SessionModel? _user;
    [ObservableProperty] private object _vistaActual;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PuedeVerInventario))]
    [NotifyPropertyChangedFor(nameof(PuedeVerCaja))]
    [NotifyPropertyChangedFor(nameof(PuedeVerMenu))]
    [NotifyPropertyChangedFor(nameof(Administrar))]
    private string _usuarioRol = "Mesero";

    public bool PuedeVerInventario => UsuarioRol == "Administrador" || UsuarioRol == "ManejoInventario";
    public bool PuedeVerCaja => UsuarioRol == "Administrador" || UsuarioRol == "Cajero";
    public bool PuedeVerMenu => UsuarioRol != "Cajero";
    public bool Administrar => UsuarioRol == "Administrador";


    public TablesViewModel(
        INavigationService navigationService,
        IViewModelFactory factory,
        GetSessionSave userSesion)
    {
        _navigationService = navigationService;
        _factory = factory;
        _userSesion = userSesion;
        _vistaActual = _factory.Create<AllTablesComponentViewModel>();
        InitializeComponent();

    }

    private void InitializeComponent()
    {
        User = _userSesion.Execute();
        UsuarioRol = User!.Rol;
    }

    [RelayCommand]
    private void ToggleTablesView()
    {
        IsShowingAllTables = !IsShowingAllTables;
        ToggleButtonText = IsShowingAllTables ? "Todas las Mesas" : "Mesas Abiertas";
        ViewTitle = IsShowingAllTables ? "Mesas Abiertas" : "Todas las Mesas";
        VistaActual = IsShowingAllTables ? _factory.Create<AllTablesComponentViewModel>() : _factory.Create<ActiveTablesComponentViewModel>();
    }


    [RelayCommand]
    public async Task AbirNuevaOrden()
    {
        var vm = _factory.Create<CantidadPaxViewModel>();
        vm.State(false);
        _navigationService.NavigateTo(vm);
    }

    [RelayCommand]
    public void Salir()
    {
        // cerrar sesion
        _navigationService.NavigateTo<LoginViewModel>();
    }

    [RelayCommand]
    public void IrAdministrar(string vista)
    {
        _navigationService.NavigateTo<AdminDashboardViewModel>();
    }
}