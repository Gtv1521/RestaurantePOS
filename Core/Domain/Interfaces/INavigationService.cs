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

        // Ahora devuelven la instancia creada
        TViewModel NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
        ViewModelBase NavigateTo(ViewModelBase viewModel);

        TViewModel ShowOverlay<TViewModel>() where TViewModel : ViewModelBase;
        ViewModelBase ShowOverlay(ViewModelBase viewModel);

        void CloseOverlay();
        bool IsOverlayOpen();
    }
}