using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiComanderaApp.Models
{
    public class ProductoModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public int Stock { get; set; }
        public double TaxPercentage { get; set; } = 18; // Default IVA 18%
        public string PrinterName { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
    }
}