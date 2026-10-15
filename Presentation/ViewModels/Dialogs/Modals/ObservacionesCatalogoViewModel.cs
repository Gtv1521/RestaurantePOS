using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Application.UseCases.Observacion;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Presentation.Views.Components.Generales;

namespace MiComanderaApp.ViewModels.Dialogs.Modals
{
    public partial class ObservacionesCatalogoViewModel : ObservableObject, IDialogViewModel<bool?>
    {
        private readonly GetAllObservacionUseCase _getTodos;
        private readonly CreateObservacionUseCase _createObservacion;
        private readonly DeleteObservacionUseCase _deleteObservacion;
        private readonly IViewModelFactory _factory;
        private bool _changed;

        public event Action<bool?>? CloseRequested;

        [ObservableProperty]
        private ObservableCollection<ObservacionModel> _observaciones = new();

        [ObservableProperty]
        private string _nuevaObservacion = string.Empty;

        [ObservableProperty]
        private decimal? _nuevoPrecio;

        [ObservableProperty]
        private bool _teclado;

        [ObservableProperty]
        private bool _botonTeclado = true;

        [ObservableProperty]
        private object? _vistaActual;

        public ObservacionesCatalogoViewModel(
            GetAllObservacionUseCase getTodos,
            CreateObservacionUseCase createObservacion,
            DeleteObservacionUseCase deleteObservacion,
            IViewModelFactory factory)
        {
            _getTodos = getTodos;
            _createObservacion = createObservacion;
            _deleteObservacion = deleteObservacion;
            _factory = factory;
            _vistaActual = _factory.Create<TecladoComponentViewModel>();
            _ = CargarAsync();
        }

        private async Task CargarAsync()
        {
            Observaciones.Clear();
            foreach (var o in await _getTodos.Execute())
                Observaciones.Add(o);
        }

        [RelayCommand]
        private async Task Agregar()
        {
            var texto = NuevaObservacion.Trim();
            if (string.IsNullOrWhiteSpace(texto)) return;

            var creado = await _createObservacion.Execute(new ObservacionRequest
            {
                Observacion = texto,
                Precio = NuevoPrecio
            });

            if (creado == null) return;

            NuevaObservacion = string.Empty;
            NuevoPrecio = null;
            _changed = true;
            await CargarAsync();
        }

        [RelayCommand]
        private async Task Quitar(ObservacionModel? observacion)
        {
            if (observacion == null) return;
            await _deleteObservacion.Execute(observacion.Id);
            _changed = true;
            await CargarAsync();
        }

        [RelayCommand]
        private void Cerrar()
        {
            CloseRequested?.Invoke(_changed);
        }

        [RelayCommand]
        private void VerTeclado()
        {
            Teclado = !Teclado;
            BotonTeclado = !BotonTeclado;
        }
    }
}
