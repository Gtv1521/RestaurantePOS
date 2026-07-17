using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using MiComanderaApp.Interfaces;
using MiComanderaApp.ViewModels;

namespace MiComanderaApp.Services.Routing
{
    public partial class NavigationService : ObservableObject, INavigationService
    {
        private readonly Func<Type, ViewModelBase> _viewModelFactory;

    // Vista principal (con animación)
    [ObservableProperty] private ViewModelBase? _currentView;

    // Vista overlay (modal flotante)
    [ObservableProperty] private ViewModelBase? _overlayView;

    [ObservableProperty]
    private bool _isOverlayVisible;

    public NavigationService(Func<Type, ViewModelBase> viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }

    // Navegación normal (con animación)
    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        CurrentView = _viewModelFactory(typeof(TViewModel));
    }

    public void NavigateTo(ViewModelBase viewModel)
    {
        CurrentView = viewModel;
    }

    // Mostrar overlay (sin animación, superpuesto)
    public void ShowOverlay<TViewModel>() where TViewModel : ViewModelBase
    {
        OverlayView = _viewModelFactory(typeof(TViewModel));
        IsOverlayVisible = true;
    }

    public void ShowOverlay(ViewModelBase viewModel)
    {
        OverlayView = viewModel;
        IsOverlayVisible = true;
    }

    // Cerrar overlay
    public void CloseOverlay()
    {
        IsOverlayVisible = false;
        OverlayView = null;
    }

    // Verificar si hay overlay abierto
    public bool IsOverlayOpen() => IsOverlayVisible;

    }
}