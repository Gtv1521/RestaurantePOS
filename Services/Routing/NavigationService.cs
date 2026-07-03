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

        [ObservableProperty]
        private ViewModelBase? _currentView;

        public NavigationService(Func<Type, ViewModelBase> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        {
            CurrentView = _viewModelFactory(typeof(TViewModel));
        }
    }
}