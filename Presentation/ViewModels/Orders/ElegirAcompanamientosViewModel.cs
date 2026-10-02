using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;

namespace MiComanderaApp.ViewModels.Orders
{
    public partial class AcompanamientoElegible : ObservableObject
    {
        [ObservableProperty]
        private AcompanamientoModel _acompanamiento = new();

        [ObservableProperty]
        private bool _seleccionado;
    }

    public partial class ElegirAcompanamientosViewModel : ObservableObject, IDialogViewModel<List<AcompanamientoModel>?>
    {
        private readonly int _maximo;

        public event Action<List<AcompanamientoModel>?>? CloseRequested;

        [ObservableProperty]
        private string _nombreProducto = string.Empty;

        [ObservableProperty]
        private ObservableCollection<AcompanamientoElegible> _opciones = new();

        public ElegirAcompanamientosViewModel(
            string nombreProducto,
            IEnumerable<AcompanamientoModel> acompanamientos,
            int maximo)
        {
            _nombreProducto = nombreProducto;
            _maximo = maximo <= 0 ? int.MaxValue : maximo;
            foreach (var a in acompanamientos)
                Opciones.Add(new AcompanamientoElegible { Acompanamiento = a });
        }

        [RelayCommand]
        private void Alternar(AcompanamientoElegible? opcion)
        {
            if (opcion == null) return;
            if (!opcion.Seleccionado && Opciones.Count(o => o.Seleccionado) >= _maximo)
                return;
            opcion.Seleccionado = !opcion.Seleccionado;
        }

        [RelayCommand]
        private void Confirmar()
        {
            CloseRequested?.Invoke(Opciones
                .Where(o => o.Seleccionado)
                .Select(o => o.Acompanamiento)
                .ToList());
        }

        [RelayCommand]
        private void Cancelar()
        {
            CloseRequested?.Invoke(null);
        }
    }
}
