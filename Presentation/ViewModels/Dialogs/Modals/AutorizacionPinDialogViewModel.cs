using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;

namespace MiComanderaApp.ViewModels.Dialogs.Modals
{
    public partial class AutorizacionPinDialogViewModel : ObservableObject, IDialogViewModel<bool>
    {
        private readonly ISession<SessionModel> _session;

        public AutorizacionPinDialogViewModel(ISession<SessionModel> session)
        {
            _session = session;
        }

        public event Action<bool>? CloseRequested;

        [ObservableProperty] private string _title = "Acceso restringido";
        [ObservableProperty] private string _message = string.Empty;
        [ObservableProperty] private string _pinCode = string.Empty;
        [ObservableProperty] private string? _error;
        [ObservableProperty] private bool _verificando;

        // Roles autorizados a validar el PIN. Por defecto Administrador o Cajero.
        private string[] _rolesPermitidos = { "Administrador", "Cajero" };
        private string _rolesPermitidosTexto = "un Administrador o Cajero";

        public void Initialize(string mensaje, string titulo = "Acceso restringido", params string[] rolesPermitidos)
        {
            Title = titulo;
            Message = mensaje;
            PinCode = string.Empty;
            Error = null;

            if (rolesPermitidos != null && rolesPermitidos.Length > 0)
            {
                _rolesPermitidos = rolesPermitidos;
                _rolesPermitidosTexto = rolesPermitidos.Length == 1
                    ? $"un {rolesPermitidos[0]}"
                    : $"un {string.Join(" o ", rolesPermitidos)}";
                RolesTexto = $"Ingresa el PIN de {_rolesPermitidosTexto}";
            }
        }

        [ObservableProperty] private string _rolesTexto = "Ingresa el PIN de un Administrador o Cajero";

        [RelayCommand]
        private void AddDigit(string digit)
        {
            if (Verificando) return;
            if (PinCode.Length < 4) PinCode += digit;

            if (PinCode.Length == 4)
                _ = VerificarAsync();
        }

        [RelayCommand]
        private void DeleteDigit()
        {
            if (Verificando) return;
            if (PinCode.Length > 0)
                PinCode = PinCode[..^1];
        }

        [RelayCommand]
        private async Task Verificar()
        {
            await VerificarAsync();
        }

        private async Task VerificarAsync()
        {
            Error = null;

            if (PinCode.Length < 4)
            {
                Error = "Ingresa el PIN de 4 dígitos.";
                return;
            }

            try
            {
                Verificando = true;

                var usuario = await _session.VerifyPinAsync(PinCode);

                if (usuario == null)
                {
                    Error = "PIN incorrecto.";
                    PinCode = string.Empty;
                    return;
                }

                if (!_rolesPermitidos.Contains(usuario.Rol))
                {
                    Error = $"El PIN no corresponde a {_rolesPermitidosTexto}.";
                    PinCode = string.Empty;
                    return;
                }

                // Acceso concedido
                CloseRequested?.Invoke(true);
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                PinCode = string.Empty;
            }
            finally
            {
                Verificando = false;
            }
        }

        [RelayCommand]
        private void Cancelar()
        {
            CloseRequested?.Invoke(false);
        }
    }
}
