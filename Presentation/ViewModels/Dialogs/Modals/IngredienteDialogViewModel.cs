using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Application.Request;
using RestaurantePOS.Core.Application.Request;

namespace MiComanderaApp.ViewModels.Dialogs.Modals
{
    public partial class IngredienteDialogViewModel : ObservableObject, IDialogViewModel<IngredienteRequest?>
    {
        public event Action<IngredienteRequest?>? CloseRequested;

        [ObservableProperty]
        private string _title = "Crear Ingrediente";
        
        [ObservableProperty]
        private string _name = string.Empty;
        
        [ObservableProperty]
        private double _initialQuantity;
        
        [ObservableProperty]
        private double _minimumQuantity;
        
        [ObservableProperty]
        private decimal _unitCost;

        [ObservableProperty]
        private string _unitOfMeasure = string.Empty;

        [RelayCommand]
        private void Guardar()
        {
            var request = new IngredienteRequest
            {
                Name = Name,
                InitialQuantity = InitialQuantity,
                MinimumQuantity = MinimumQuantity,
                UnitCost = UnitCost,
                UnitOfMeasure = UnitOfMeasure
            };
            CloseRequested?.Invoke(request);
        }

        [RelayCommand]
        private void Cancelar()
        {
            CloseRequested?.Invoke(null);
        }
    }
}
