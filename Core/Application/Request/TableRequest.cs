using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiComanderaApp.Core.Application.Request
{
    public class TableRequest
    {
        public int NumeroMesa { get; set; }
        public int Capacidad { get; set; }
        public int Estado { get; set; }
        public string Ubicacion { get; set; } = "";
        public bool Activo { get; set; } = false;
    }
}