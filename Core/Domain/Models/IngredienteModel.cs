
using System.Text.Json.Serialization;

namespace MiComanderaApp.Core.Domain.Models
{
    public class IngredienteModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("availableQuantity")]
        public double AvailableQuantity { get; set; }

        [JsonPropertyName("minimumQuantity")]
        public double MinimumQuantity { get; set; }

        [JsonPropertyName("unitCost")]
        public decimal UnitCost { get; set; }

        [JsonPropertyName("unitOfMeasure")]
        public string UnitOfMeasure { get; set; } = "";

        public string StatusMessage
        {
            get
            {
                if (AvailableQuantity <= 0)
                {
                    return "Agotado";
                }
                if (AvailableQuantity <= MinimumQuantity)
                {
                    return $"Stock bajo (Mínimo: {MinimumQuantity})";
                }
                return "Stock saludable";
            }
        }
    }
}
