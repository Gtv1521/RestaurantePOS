using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Presentation.Views.Components.Generales;
using MiComanderaApp.Request;

namespace MiComanderaApp.ViewModels.Dialogs.Modals
{
    public partial class CreateUserViewModel : ObservableObject, IDialogViewModel<UserRequest?>
    {
        private readonly IViewModelFactory _factory;

        public CreateUserViewModel(IViewModelFactory factory)
        {
            _factory = factory;
            _vistaActual = _factory.Create<TecladoComponentViewModel>();
        }

        public event Action<UserRequest?>? CloseRequested;

        [ObservableProperty]
        private string _title = "Agregar Usuario";

        [ObservableProperty]
        private bool _teclado;

        [ObservableProperty]
        private bool _botonTeclado = true;

        [ObservableProperty]
        private object? _vistaActual;

        [ObservableProperty]
        private string _nombreCompleto = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private int _pin;

        [ObservableProperty]
        private string? _rolSeleccionado;

        [ObservableProperty]
        private bool _activo = true;

        [ObservableProperty]
        private string? _error;

        public List<string> Roles { get; } = new()
        {
            "Administrador",
            "Mesero",
            "Cajero",
            "Inventario"
        };

        [RelayCommand]
        private void Guardar()
        {
            Error = null;

            if (string.IsNullOrWhiteSpace(NombreCompleto) ||
                string.IsNullOrWhiteSpace(Email) ||
                Pin == 0 ||
                RolSeleccionado == null)
            {
                Error = "Completa todos los campos obligatorios.";
                return;
            }

            var request = new UserRequest
            {
                NombreCompleto = NombreCompleto,
                Email = Email,
                Pin = Pin,
                Rol = Roles.IndexOf(RolSeleccionado),
                Activo = Activo
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
