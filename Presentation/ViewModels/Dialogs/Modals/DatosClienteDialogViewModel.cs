using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;

namespace MiComanderaApp.ViewModels.Dialogs.Modals
{
    public partial class DatosClienteDialogViewModel : ObservableObject, IDialogViewModel<VentaClienteRequest?>
    {
        public event Action<VentaClienteRequest?>? CloseRequested;

        [ObservableProperty] private string _title = "Datos del Cliente";
        [ObservableProperty] private string _clienteNombre = string.Empty;
        [ObservableProperty] private string _clienteDocumento = string.Empty;
        [ObservableProperty] private string _clienteDireccion = string.Empty;
        [ObservableProperty] private string _clienteTelefono = string.Empty;
        [ObservableProperty] private string? _error;

        public void Initialize(VentaDetalleModel venta)
        {
            ClienteNombre = venta.ClienteNombre ?? string.Empty;
            ClienteDocumento = venta.ClienteDocumento ?? string.Empty;
            ClienteDireccion = venta.ClienteDireccion ?? string.Empty;
            ClienteTelefono = venta.ClienteTelefono ?? string.Empty;
        }

        [RelayCommand]
        private void Guardar()
        {
            Error = null;

            if (string.IsNullOrWhiteSpace(ClienteNombre))
            {
                Error = "El nombre del cliente es obligatorio.";
                return;
            }

            CloseRequested?.Invoke(new VentaClienteRequest
            {
                ClienteNombre = ClienteNombre.Trim(),
                ClienteDocumento = ClienteDocumento.Trim(),
                ClienteDireccion = ClienteDireccion.Trim(),
                ClienteTelefono = ClienteTelefono.Trim()
            });
        }

        [RelayCommand]
        private void Cancelar()
        {
            CloseRequested?.Invoke(null);
        }
    }
}
