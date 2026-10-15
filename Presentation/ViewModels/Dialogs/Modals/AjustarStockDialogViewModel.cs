using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Presentation.Views.Components.Generales;

namespace MiComanderaApp.ViewModels.Dialogs.Modals
{
    public class AjustarStockResult
    {
        public decimal NewQuantity { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public partial class AjustarStockDialogViewModel : ObservableObject, IDialogViewModel<AjustarStockResult?>
    {
        private readonly IViewModelFactory _factory;

        public AjustarStockDialogViewModel(IViewModelFactory factory)
        {
            _factory = factory;
            _vistaActual = _factory.Create<TecladoComponentViewModel>();
        }

        public event Action<AjustarStockResult?>? CloseRequested;

        [ObservableProperty] private string _title = "Ajustar Stock";
        [ObservableProperty] private string _ingredienteNombre = string.Empty;
        [ObservableProperty] private double _cantidadActual;
        [ObservableProperty] private string _unidadMedida = string.Empty;
        [ObservableProperty] private decimal _nuevaCantidad;
        [ObservableProperty] private string _motivo = string.Empty;
        [ObservableProperty] private string? _error;
        [ObservableProperty] private bool _teclado;
        [ObservableProperty] private bool _botonTeclado = true;
        [ObservableProperty] private object? _vistaActual;

        public void Initialize(IngredienteModel ingrediente)
        {
            Title = $"Ajustar Stock - {ingrediente.Name}";
            IngredienteNombre = ingrediente.Name;
            CantidadActual = ingrediente.AvailableQuantity;
            UnidadMedida = ingrediente.UnitOfMeasure;
            NuevaCantidad = (decimal)ingrediente.AvailableQuantity;
        }

        [RelayCommand]
        private void Guardar()
        {
            Error = null;

            if (NuevaCantidad < 0)
            {
                Error = "La cantidad no puede ser negativa.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Motivo))
            {
                Error = "Indica el motivo del ajuste.";
                return;
            }

            CloseRequested?.Invoke(new AjustarStockResult
            {
                NewQuantity = NuevaCantidad,
                Reason = Motivo.Trim()
            });
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
