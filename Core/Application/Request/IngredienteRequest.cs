
using System.Text.Json.Serialization;

namespace MiComanderaApp.Core.Application.Request
{
    public class IngredienteRequest
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("initialQuantity")]
        public double InitialQuantity { get; set; }

        [JsonPropertyName("minimumQuantity")]
        public double MinimumQuantity { get; set; }

        [JsonPropertyName("unitCost")]
        public decimal UnitCost { get; set; }

        [JsonPropertyName("unitOfMeasure")]
        public string UnitOfMeasure { get; set; } = "";
    }
}
