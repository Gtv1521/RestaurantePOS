using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiComanderaApp.Core.Domain.Models
{
    public class ObservacionModel
    {
        public int Id { get; set; }
        public string Observacion { get; set; } = string.Empty;
        public decimal? Precio { get; set; }

        public bool TienePrecio => Precio.HasValue && Precio.Value > 0;
        public string PrecioTexto => TienePrecio ? $"+ ${Precio:0.00}" : string.Empty;
    }
}