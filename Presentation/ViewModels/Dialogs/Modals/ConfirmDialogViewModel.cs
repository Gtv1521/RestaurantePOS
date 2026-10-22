using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Domain.Interfaces;

namespace MiComanderaApp.ViewModels.Dialogs.Modals
{
    public partial class ConfirmDialogViewModel : ObservableObject, IDialogViewModel<bool>
    {
        public ConfirmDialogViewModel()
        {
        }

        public event Action<bool>? CloseRequested;

        [ObservableProperty] private string _title = "Confirmar";
        [ObservableProperty] private string _message = string.Empty;
        [ObservableProperty] private string _textoConfirmar = "Sí, anular";
        [ObservableProperty] private string _textoCancelar = "Cancelar";

        public void Initialize(string titulo, string mensaje, string? textoConfirmar = null, string? textoCancelar = null)
        {
            Title = titulo;
            Message = mensaje;
            TextoConfirmar = string.IsNullOrWhiteSpace(textoConfirmar) ? "Sí, anular" : textoConfirmar;
            TextoCancelar = string.IsNullOrWhiteSpace(textoCancelar) ? "Cancelar" : textoCancelar;
        }

        [RelayCommand]
        private void Confirmar()
        {
            CloseRequested?.Invoke(true);
        }

        [RelayCommand]
        private void Cancelar()
        {
            CloseRequested?.Invoke(false);
        }
    }
}
