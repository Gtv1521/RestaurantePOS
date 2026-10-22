using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.UseCases.Print;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;

namespace MiComanderaApp.ViewModels.Dialogs.Modals
{
    public partial class PrintPreviewDialogViewModel : ObservableObject, IDialogViewModel<bool?>
    {
        private readonly GetImpresorasUseCase _getImpresoras;
        private readonly ImprimirDocumentoUseCase _imprimir;
        private bool _impreso;

        public event Action<bool?>? CloseRequested;

        [ObservableProperty] private string _title = "Vista previa de impresión";
        [ObservableProperty] private string _texto = string.Empty;
        [ObservableProperty] private string? _mensaje;
        [ObservableProperty] private bool _mensajeError;
        [ObservableProperty] private bool _procesando;
        [ObservableProperty] private bool _cargandoImpresoras;
        [ObservableProperty] private ImpresoraModel? _impresoraSeleccionada;

        public ObservableCollection<ImpresoraModel> Impresoras { get; } = new();

        public bool MensajeOk => !MensajeError && !string.IsNullOrEmpty(Mensaje);

        partial void OnMensajeChanged(string? value)
        {
            OnPropertyChanged(nameof(MensajeOk));
        }

        partial void OnMensajeErrorChanged(bool value)
        {
            OnPropertyChanged(nameof(MensajeOk));
        }

        public PrintPreviewDialogViewModel(
            GetImpresorasUseCase getImpresoras,
            ImprimirDocumentoUseCase imprimir)
        {
            _getImpresoras = getImpresoras;
            _imprimir = imprimir;
        }

        public void Initialize(string titulo, string texto)
        {
            Title = titulo;
            Texto = texto;
            Mensaje = null;
            _impreso = false;
            _ = CargarImpresorasAsync();
        }

        [RelayCommand]
        private async Task CargarImpresorasAsync()
        {
            CargandoImpresoras = true;
            try
            {
                var impresoras = await _getImpresoras.Execute();
                Impresoras.Clear();
                foreach (var impresora in impresoras.Where(i => i.Active))
                {
                    Impresoras.Add(impresora);
                }
                ImpresoraSeleccionada = Impresoras.FirstOrDefault();
                MensajeError = false;
                Mensaje = Impresoras.Count == 0 ? "No hay impresoras configuradas. Podés copiar el documento." : null;
            }
            catch (Exception ex)
            {
                MensajeError = true;
                Mensaje = $"No se pudieron cargar las impresoras: {ex.Message}";
            }
            finally
            {
                CargandoImpresoras = false;
            }
        }

        [RelayCommand]
        private async Task Cerrar()
        {
            if (!_impreso)
            {
                CloseRequested?.Invoke(null);
            }
            else
            {
                CloseRequested?.Invoke(true);
            }
        }
        [RelayCommand]
        private async Task Imprimir()
        {
            if (ImpresoraSeleccionada == null)
            {
                MensajeError = true;
                Mensaje = "No hay impresora seleccionada.";
                return;
            }

            Procesando = true;
            try
            {
                await _imprimir.Execute(ImpresoraSeleccionada.Id, Texto);
                _impreso = true;
                MensajeError = false;
                Mensaje = "Documento impreso correctamente.";
            }
            catch (Exception ex)
            {
                MensajeError = true;
                Mensaje = $"Error al imprimir: {ex.Message}";
            }
            finally
            {
                Procesando = false;
            }
        }
    }

}
