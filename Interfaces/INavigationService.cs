using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.ViewModels;

namespace MiComanderaApp.Interfaces
{
    public interface INavigationService
    {
        ViewModelBase? CurrentView { get; }
        void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
    }
}