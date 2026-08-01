using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiComanderaApp.Core.Application.Request
{
    public class ObservacionRequest
    {
        public string Observacion { get; set; } = string.Empty;
        public decimal? Precio { get; set; }
    }
}