using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;

namespace MiComanderaApp.ViewModels.Orders
{
    public partial class ElegirTerminoViewModel : ObservableObject, IDialogViewModel<TerminoModel?>
    {
        public event Action<TerminoModel?>? CloseRequested;

        [ObservableProperty]
        private string _nombreProducto = string.Empty;

        [ObservableProperty]
        private ObservableCollection<TerminoModel> _terminos = new();

        [ObservableProperty]
        private TerminoModel? _terminoSeleccionado;

        public ElegirTerminoViewModel(string nombreProducto, System.Collections.Generic.IEnumerable<TerminoModel> terminos)
        {
            _nombreProducto = nombreProducto;
            foreach (var t in terminos)
                Terminos.Add(t);
            TerminoSeleccionado = Terminos.FirstOrDefault();
        }

        [RelayCommand]
        private void Confirmar()
        {
            CloseRequested?.Invoke(TerminoSeleccionado);
        }

        [RelayCommand]
        private void Cancelar()
        {
            CloseRequested?.Invoke(null);
        }
    }
}
