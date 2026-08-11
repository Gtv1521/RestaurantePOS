
using System.Text.Json.Serialization;

namespace MiComanderaApp.Core.Domain.Models
{
    public class IngredienteModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double AvailableQuantity { get; set; }
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
