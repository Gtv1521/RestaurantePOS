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
        
        ViewModelBase? OverlayView { get; }
        bool IsOverlayVisible { get; }  

        void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
        void NavigateTo(ViewModelBase viewModel);

        void ShowOverlay<TViewModel>() where TViewModel : ViewModelBase;
        void ShowOverlay(ViewModelBase viewModel);
        void CloseOverlay();
        bool IsOverlayOpen();
    }
}