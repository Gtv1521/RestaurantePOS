using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;

namespace MiComanderaApp.ViewModels.Dialogs.Modals
{
    public partial class MetodoPagoOption : ObservableObject
    {
        public string Nombre { get; init; } = string.Empty;
        public int Codigo { get; init; }
        public string? MarcaTarjeta { get; init; }
        public string Letra { get; init; } = string.Empty;
        public string Etiqueta { get; init; } = string.Empty;

        [ObservableProperty] private bool _seleccionado;
    }

    public class PagoItemModel
    {
        public string MetodoNombre { get; init; } = string.Empty;
        public decimal Monto { get; init; }
        public string MontoTexto => $"S/ {Monto:N2}";
    }

    public class CobroResult
    {
        public decimal Propina { get; set; }
        public List<PagoRequest> Pagos { get; set; } = new();
    }

    public partial class CobrarDialogViewModel : ObservableObject, IDialogViewModel<CobroResult?>
    {
        public event Action<CobroResult?>? CloseRequested;

        [ObservableProperty] private string _title = "Cobrar Venta";
        [ObservableProperty] private int _numeroMesa;
        [ObservableProperty] private decimal _total;
        [ObservableProperty] private decimal _propina;
        [ObservableProperty] private decimal _montoPago;
        [ObservableProperty] private MetodoPagoOption? _metodoSeleccionado;
        [ObservableProperty] private string? _error;
        [ObservableProperty] private bool _procesando;

        public ObservableCollection<MetodoPagoOption> Metodos { get; } = new()
        {
            new() { Nombre = "Efectivo", Codigo = 0, MarcaTarjeta = null, Letra = "💵" },
            new() { Nombre = "Visa", Codigo = 1, MarcaTarjeta = "Visa", Letra = "V" },
            new() { Nombre = "Mastercard", Codigo = 1, MarcaTarjeta = "Mastercard", Letra = "M" },
            new() { Nombre = "American Express", Codigo = 1, MarcaTarjeta = "American Express", Letra = "AM" },
            new() { Nombre = "Otra tarjeta", Codigo = 2, MarcaTarjeta = "Otra tarjeta", Letra = "T" },
            new() { Nombre = "Transferencia", Codigo = 3, MarcaTarjeta = null, Letra = "↗" },
        };

        public ObservableCollection<PagoItemModel> PagosAgregados { get; } = new();

        public decimal TotalPagado => PagosAgregados.Sum(p => p.Monto);
        public string TotalTexto => $"S/ {Total:N2}";
        public string PagadoTexto => $"S/ {TotalPagado:N2}";
        public string PendienteTexto => $"S/ {Math.Max(0, Total + Propina - TotalPagado):N2}";
        public bool PuedeCobrar => PagosAgregados.Count > 0 && TotalPagado >= Total + Propina && !Procesando;

        public CobrarDialogViewModel()
        {
        }

        public void Initialize(VentaDetalleModel venta)
        {
            NumeroMesa = venta.NumeroMesa;
            Total = venta.TotalAmount;
            Propina = 0;
            PagosAgregados.Clear();
            SeleccionarMetodoInterno(Metodos.First());
            MontoPago = Total;
            OnPropertyChanged(nameof(TotalTexto));
            OnPropertyChanged(nameof(PendienteTexto));
            ActualizarResumen();
        }

        [RelayCommand]
        private void SeleccionarMetodo(MetodoPagoOption? metodo)
        {
            if (metodo == null) return;
            SeleccionarMetodoInterno(metodo);
        }

        private void SeleccionarMetodoInterno(MetodoPagoOption metodo)
        {
            foreach (var m in Metodos)
                m.Seleccionado = m == metodo;

            MetodoSeleccionado = metodo;
        }

        partial void OnPropinaChanged(decimal value) => ActualizarResumen();

        partial void OnMetodoSeleccionadoChanged(MetodoPagoOption? value)
        {
            MontoPago = Math.Max(0, Total + Propina - TotalPagado);
        }

        [RelayCommand]
        private void AgregarPago()
        {
            Error = null;

            if (MetodoSeleccionado == null)
            {
                Error = "Selecciona un método de pago.";
                return;
            }

            if (MontoPago <= 0)
            {
                Error = "El monto debe ser mayor a 0.";
                return;
            }

            PagosAgregados.Add(new PagoItemModel
            {
                MetodoNombre = MetodoSeleccionado.MarcaTarjeta ?? MetodoSeleccionado.Nombre,
                Monto = MontoPago
            });

            MontoPago = Math.Max(0, Total + Propina - TotalPagado);
            ActualizarResumen();
        }

        [RelayCommand]
        private void QuitarPago(PagoItemModel? pago)
        {
            if (pago == null) return;
            PagosAgregados.Remove(pago);
            MontoPago = Total + Propina - TotalPagado;
            ActualizarResumen();
        }

        [RelayCommand]
        private void Confirmar()
        {
            Error = null;

            if (PagosAgregados.Count == 0)
            {
                Error = "Agrega al menos un pago.";
                return;
            }

            if (TotalPagado < Total + Propina)
            {
                Error = "El monto pagado es insuficiente para cerrar la venta.";
                return;
            }

            // Reconstruye los pagos agrupados por método/marca
            var pagos = PagosAgregados
                .GroupBy(p => p.MetodoNombre)
                .Select(g =>
                {
                    var metodo = Metodos.FirstOrDefault(m =>
                                    (m.MarcaTarjeta ?? m.Nombre) == g.Key)
                                ?? Metodos.First();
                    return new PagoRequest
                    {
                        Metodo = metodo.Codigo,
                        MarcaTarjeta = metodo.MarcaTarjeta,
                        Monto = g.Sum(x => x.Monto),
                        Referencia = metodo.MarcaTarjeta == null ? null : $"**** {DateTime.Now:HHmm}"
                    };
                })
                .ToList();

            CloseRequested?.Invoke(new CobroResult
            {
                Propina = Propina,
                Pagos = pagos
            });
        }

        [RelayCommand]
        private void Cancelar()
        {
            CloseRequested?.Invoke(null);
        }

        private void ActualizarResumen()
        {
            OnPropertyChanged(nameof(TotalPagado));
            OnPropertyChanged(nameof(PagadoTexto));
            OnPropertyChanged(nameof(PendienteTexto));
            OnPropertyChanged(nameof(PuedeCobrar));
        }
    }
}
