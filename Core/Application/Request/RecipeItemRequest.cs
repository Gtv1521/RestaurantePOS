using System.Text.Json.Serialization;

namespace MiComanderaApp.Core.Application.Request
{
    public class RecipeItemRequest
    {
        [JsonPropertyName("ingredientId")]
        public int IngredientId { get; set; }

        [JsonPropertyName("quantityNeeded")]
        public double Quantity { get; set; }
    }
}
