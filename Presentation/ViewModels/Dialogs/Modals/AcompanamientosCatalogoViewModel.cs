using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Application.UseCases.Acompanamiento;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Presentation.Views.Components.Generales;

namespace MiComanderaApp.ViewModels.Dialogs.Modals
{
    public partial class AcompanamientosCatalogoViewModel : ObservableObject, IDialogViewModel<bool?>
    {
        private readonly GetAllAcompanamientosUseCase _getTodos;
        private readonly CreateAcompanamientoUseCase _create;
        private readonly DeleteAcompanamientoUseCase _delete;
        private readonly IViewModelFactory _factory;
        private bool _changed;

        public event Action<bool?>? CloseRequested;

        [ObservableProperty]
        private ObservableCollection<AcompanamientoModel> _acompanamientos = new();

        [ObservableProperty]
        private string _nuevoNombre = string.Empty;

        [ObservableProperty]
        private string _nuevaDescripcion = string.Empty;

        [ObservableProperty]
        private bool _teclado;

        [ObservableProperty]
        private bool _botonTeclado = true;

        [ObservableProperty]
        private object? _vistaActual;

        public AcompanamientosCatalogoViewModel(
            GetAllAcompanamientosUseCase getTodos,
            CreateAcompanamientoUseCase create,
            DeleteAcompanamientoUseCase delete,
            IViewModelFactory factory)
        {
            _getTodos = getTodos;
            _create = create;
            _delete = delete;
            _factory = factory;
            _vistaActual = _factory.Create<TecladoComponentViewModel>();
            _ = CargarAsync();
        }

        private async Task CargarAsync()
        {
            Acompanamientos.Clear();
            foreach (var a in await _getTodos.Execute())
                Acompanamientos.Add(a);
        }

        [RelayCommand]
        private async Task Agregar()
        {
            var nombre = NuevoNombre.Trim();
            if (string.IsNullOrWhiteSpace(nombre)) return;

            var creado = await _create.Execute(new AcompanamientoRequest
            {
                Name = nombre,
                Description = NuevaDescripcion.Trim()
            });
            if (creado == null) return;

            NuevoNombre = string.Empty;
            NuevaDescripcion = string.Empty;
            _changed = true;
            await CargarAsync();
        }

        [RelayCommand]
        private async Task Quitar(AcompanamientoModel? acompanamiento)
        {
            if (acompanamiento == null) return;
            await _delete.Execute(acompanamiento.Id);
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
