using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Application.UseCases.Termino;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Presentation.Views.Components.Generales;

namespace MiComanderaApp.ViewModels.Dialogs.Modals
{
    public partial class TerminosCatalogoViewModel : ObservableObject, IDialogViewModel<bool?>
    {
        private readonly GetAllTerminosUseCase _getTodos;
        private readonly CreateTerminoUseCase _createTermino;
        private readonly DeleteTerminoUseCase _deleteTermino;
        private readonly IViewModelFactory _factory;
        private bool _changed;

        public event Action<bool?>? CloseRequested;

        [ObservableProperty]
        private ObservableCollection<TerminoModel> _terminos = new();

        [ObservableProperty]
        private string _nuevoTermino = string.Empty;

        [ObservableProperty]
        private bool _teclado;

        [ObservableProperty]
        private bool _botonTeclado = true;

        [ObservableProperty]
        private object? _vistaActual;

        public TerminosCatalogoViewModel(
            GetAllTerminosUseCase getTodos,
            CreateTerminoUseCase createTermino,
            DeleteTerminoUseCase deleteTermino,
            IViewModelFactory factory)
        {
            _getTodos = getTodos;
            _createTermino = createTermino;
            _deleteTermino = deleteTermino;
            _factory = factory;
            _vistaActual = _factory.Create<TecladoComponentViewModel>();
            _ = CargarAsync();
        }

        private async Task CargarAsync()
        {
            Terminos.Clear();
            foreach (var t in await _getTodos.Execute())
                Terminos.Add(t);
        }

        [RelayCommand]
        private async Task Agregar()
        {
            var texto = NuevoTermino.Trim();
            if (string.IsNullOrWhiteSpace(texto)) return;

            var creado = await _createTermino.Execute(new TerminoRequest { Termino = texto });
            if (creado == null) return;

            NuevoTermino = string.Empty;
            _changed = true;
            await CargarAsync();
        }

        [RelayCommand]
        private async Task Quitar(TerminoModel? termino)
        {
            if (termino == null) return;
            await _deleteTermino.Execute(termino.Id);
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
