using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MiComanderaApp.Core.Application.Request
{
    public class RecetaRequest
    {
        [JsonPropertyName("productId")]
        public int ProductId { get; set; }
        [JsonPropertyName("productName")]
        public string ProductName { get; set; } = string.Empty;
        [JsonPropertyName("recipeItems")]
        public List<RecipeItemRequest> RecipeItems { get; set; } = new List<RecipeItemRequest>();

    }
}
