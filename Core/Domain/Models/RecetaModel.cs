using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MiComanderaApp.Core.Domain.Models;

namespace RestaurantePOS.Core.Domain.Models
{
    public class RecetaModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        [JsonPropertyName("items")]
        public List<IngredientModel> Items { get; set; } = new List<IngredientModel>();
    }
    public class IngredientModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("ingredientName")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("quantityNeeded")]
        public double Quantity { get; set; }
        [JsonPropertyName("unitOfMeasure")]
        public string UnitOfMeasure { get; set; } = string.Empty;
    }
}