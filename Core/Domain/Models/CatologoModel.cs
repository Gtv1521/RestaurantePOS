using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiComanderaApp.Core.Domain.Models
{
    public class CatalogoModel
    {

        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public bool Active { get; set; }
        public int ProductsCount { get; set; }
        public int? TiempoPreparacion { get; set; }
    }
}