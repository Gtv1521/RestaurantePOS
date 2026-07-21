using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantePOS.Core.Application.Request
{
    public class IngredienteRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double InitialQuantity { get; set; }
        public double MinimumQuantity { get; set; }
        public decimal UnitCost { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty;
    }
}