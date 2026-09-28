using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;
using System;

namespace MiComanderaApp.ViewModels.Dialogs.Modals
{
    public partial class EditIngredienteDialogViewModel : ObservableObject, IDialogViewModel<IngredienteRequest?>
    {
        public event Action<IngredienteRequest?>? CloseRequested;

        [ObservableProperty]
        private string _title = "Editar Ingrediente";

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private double _currentQuantity;

        [ObservableProperty]
        private double _minimumQuantity;
        
        [ObservableProperty]
        private decimal _unitCost;

        [ObservableProperty]
        private string _unitOfMeasure;

        [ObservableProperty]
        private double _quantityToAdd;

        [ObservableProperty]
        private double _quantityToRemove;
        
        private readonly int _ingredienteId;

        public EditIngredienteDialogViewModel(IngredienteModel ingrediente)
        {
            _ingredienteId = ingrediente.Id;
            _name = ingrediente.Name;
            _currentQuantity = ingrediente.AvailableQuantity;
            _minimumQuantity = ingrediente.MinimumQuantity;
            _unitCost = ingrediente.UnitCost;
            _unitOfMeasure = ingrediente.UnitOfMeasure;
        }

        [RelayCommand]
        private void Guardar()
        {
            var newQuantity = CurrentQuantity + QuantityToAdd - QuantityToRemove;

            var request = new IngredienteRequest
            {
                Id = _ingredienteId,
                Name = Name,
                InitialQuantity = newQuantity, // La cantidad inicial en el request será la nueva cantidad disponible actualizada
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
