using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace MiComanderaApp.Models
{
    public class ProductoModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public double TaxPercentage { get; set; } = 18; // Default IVA 18%
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string[] Accompaniments { get; set; } = [];
    }
}