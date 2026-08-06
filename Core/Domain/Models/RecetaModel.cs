using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Domain.Models;

namespace RestaurantePOS.Core.Domain.Models
{
    public class RecetaModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public List<IngredienteModel> Items { get; set; } = new List<IngredienteModel>();
    }
}