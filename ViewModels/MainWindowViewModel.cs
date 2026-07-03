﻿using CommunityToolkit.Mvvm.ComponentModel;
using MiComanderaApp.Interfaces;
using MiComanderaApp.ViewModels.Orders;
using Microsoft.Extensions.DependencyInjection;

namespace MiComanderaApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public INavigationService Navigation { get; }

    public MainWindowViewModel(INavigationService navigationService)
    {
        Navigation = navigationService;
        Navigation.NavigateTo<LoginViewModel>(); // se define el primer ViewModel que se mostrará al iniciar la aplicación
    }
}
