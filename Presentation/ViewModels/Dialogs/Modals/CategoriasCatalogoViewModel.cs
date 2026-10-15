using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Application.UseCases.Catalogo;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Presentation.Views.Components.Generales;

namespace MiComanderaApp.ViewModels.Dialogs.Modals
{
    public partial class CategoriasCatalogoViewModel : ObservableObject, IDialogViewModel<bool?>
    {
        private readonly GetAllCatalogoUseCase _getTodos;
        private readonly CreateCatalogoUseCase _createCatalogo;
        private readonly DeleteCatalogoUseCase _deleteCatalogo;
        private readonly IViewModelFactory _factory;
        private bool _changed;

        public event Action<bool?>? CloseRequested;

        [ObservableProperty]
        private ObservableCollection<CatalogoModel> _categorias = new();

        [ObservableProperty]
        private string _nuevoNombre = string.Empty;

        [ObservableProperty]
        private string _nuevaDescripcion = string.Empty;

        [ObservableProperty]
        private int _tiempoPreparacion;

        [ObservableProperty]
        private bool _teclado;

        [ObservableProperty]
        private bool _botonTeclado = true;

        [ObservableProperty]
        private object? _vistaActual;

        public CategoriasCatalogoViewModel(
            GetAllCatalogoUseCase getTodos,
            CreateCatalogoUseCase createCatalogo,
            DeleteCatalogoUseCase deleteCatalogo,
            IViewModelFactory factory)
        {
            _getTodos = getTodos;
            _createCatalogo = createCatalogo;
            _deleteCatalogo = deleteCatalogo;
            _factory = factory;
            _vistaActual = _factory.Create<TecladoComponentViewModel>();
            _ = CargarAsync();
        }

        private async Task CargarAsync()
        {
            Categorias.Clear();
            foreach (var c in await _getTodos.Execute())
                Categorias.Add(c);
        }

        [RelayCommand]
        private async Task Agregar()
        {
            var nombre = NuevoNombre.Trim();
            if (string.IsNullOrWhiteSpace(nombre)) return;

            var id = await _createCatalogo.Execute(new CatalogoRequest
            {
                Name = nombre,
                Description = NuevaDescripcion.Trim(),
                TiempoPreparacion = TiempoPreparacion
            });

            if (id == null) return;

            NuevoNombre = string.Empty;
            NuevaDescripcion = string.Empty;
            TiempoPreparacion = 0;
            _changed = true;
            await CargarAsync();
        }

        [RelayCommand]
        private async Task Quitar(CatalogoModel? categoria)
        {
            if (categoria == null) return;
            await _deleteCatalogo.Execute(categoria.Id);
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
