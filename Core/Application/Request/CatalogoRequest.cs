using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiComanderaApp.Core.Application.Request
{
    public class CatalogoRequest
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int TiempoPreparacion { get; set; } 
    }
}