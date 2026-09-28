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
        private readonly IViewModelFactory _factory;
        private readonly IServiceProvider _services;


        // private readonly Func<Type, ViewModelBase> _viewModelFactory;

        // Vista principal (con animación)
        [ObservableProperty] private ViewModelBase? _currentView;
        [ObservableProperty] private ViewModelBase? _overlayView;
        [ObservableProperty] private bool _isOverlayVisible;

        public NavigationService(IViewModelFactory factory, IServiceProvider services)
        {
            _factory = factory;
            _services = services;
        }

        // Navegación normal (con animación)
        public TViewModel NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        {
            var vm = _factory.Create<TViewModel>();
            CurrentView = vm;
            return vm;
        }

        public ViewModelBase NavigateTo(ViewModelBase viewModel)
        {
            CurrentView = viewModel;
            return viewModel;
        }

        // Mostrar overlay (sin animación, superpuesto)
        public TViewModel ShowOverlay<TViewModel>() where TViewModel : ViewModelBase
        {
            var vm = _factory.Create<TViewModel>();
            OverlayView = vm;
            return vm;
        }

        public ViewModelBase ShowOverlay(ViewModelBase viewModel)
        {
            OverlayView = viewModel;
            return viewModel;
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