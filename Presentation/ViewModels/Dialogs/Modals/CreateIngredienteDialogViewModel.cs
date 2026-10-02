using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Presentation.Views.Components.Generales;

namespace MiComanderaApp.ViewModels.Dialogs.Modals
{
    public partial class CreateIngredienteDialogViewModel : ObservableObject, IDialogViewModel<IngredienteModel?>
    {
        private readonly IViewModelFactory _factory;

        public CreateIngredienteDialogViewModel(IViewModelFactory factory)
        {
            _factory = factory;
            _vistaActual = _factory.Create<TecladoComponentViewModel>();
        }

        public event Action<IngredienteModel?>? CloseRequested;

        [ObservableProperty]
        private string _title = "Crear Ingrediente";

        [ObservableProperty]
        private bool _teclado;

        [ObservableProperty]
        private bool _botonTeclado = true;

        [ObservableProperty]
        private object? _vistaActual;
        
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
            var request = new IngredienteModel
            {
                Name = Name,
                AvailableQuantity = InitialQuantity,
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

        [RelayCommand]
        private void VerTeclado()
        {
            Teclado = !Teclado;
            BotonTeclado = !BotonTeclado;
        }
    }
}
