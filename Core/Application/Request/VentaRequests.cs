using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiComanderaApp.Core.Application.Request
{
    public class VentaClienteRequest
    {
        [StringLength(120)]
        public string? ClienteNombre { get; set; }

        [StringLength(30)]
        public string? ClienteDocumento { get; set; }

        [StringLength(160)]
        public string? ClienteDireccion { get; set; }

        [StringLength(40)]
        public string? ClienteTelefono { get; set; }
    }

    public class CerrarVentaRequest
    {
        public int VentaId { get; set; }
        public decimal Propina { get; set; } = 0;
        public List<PagoRequest> Pagos { get; set; } = new();
    }

    public class PagoRequest
    {
        /// <summary>0=Efectivo, 1=TarjetaCredito, 2=TarjetaDebito, 3=Transferencia</summary>
        public int Metodo { get; set; }
        public decimal Monto { get; set; }
        public string? MarcaTarjeta { get; set; }
        public string? Referencia { get; set; }
    }
}
