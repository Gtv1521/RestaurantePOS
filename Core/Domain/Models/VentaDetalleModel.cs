using System;
using System.Collections.Generic;
using System.Linq;

namespace MiComanderaApp.Core.Domain.Models
{
    public class VentaDetalleModel
    {
        public int VentaId { get; set; }
        public int MesaId { get; set; }
        public int NumeroMesa { get; set; }
        public int Instancia { get; set; }
        public string? AliasVenta { get; set; }
        public string NombreMesero { get; set; } = string.Empty;
        public DateTime FechaApertura { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal IVA { get; set; }
        public decimal TotalAmount { get; set; }
        public string? ClienteNombre { get; set; }
        public string? ClienteDocumento { get; set; }
        public string? ClienteDireccion { get; set; }
        public string? ClienteTelefono { get; set; }
        public List<ComandaDetalleModel> Comandas { get; set; } = new();
        public List<PagoVentaModel> Pagos { get; set; } = new();

        public bool TieneCliente => !string.IsNullOrWhiteSpace(ClienteNombre);
        public bool TienePagos => Pagos.Any();
        public decimal TotalPagado => Pagos.Sum(p => p.Monto);
        public string ClienteTexto => TieneCliente
            ? $"{ClienteNombre} {(string.IsNullOrWhiteSpace(ClienteDocumento) ? "" : $"• {ClienteDocumento}")}"
            : "Sin datos de cliente";
    }

    public class ComandaDetalleModel
    {
        public int ComandaId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public List<ComandaItemDetalleModel> Items { get; set; } = new();
        public decimal Subtotal => Items.Sum(i => i.Subtotal);
    }

    public class ComandaItemDetalleModel
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string? Termino { get; set; }
        public List<int> AcompanamientoIds { get; set; } = new();
        public List<int> Observaciones { get; set; } = new();
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }

    public class PagoVentaModel
    {
        public int Id { get; set; }
        public string Metodo { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string MarcaTarjeta { get; set; } = string.Empty;
        public string Referencia { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
    }
}
