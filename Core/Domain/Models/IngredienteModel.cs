namespace MiComanderaApp.Core.Domain.Models
{
    public class IngredienteModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double InitialQuantity { get; set; }
        public double MinimumQuantity { get; set; }
        public decimal UnitCost { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty;
    }
}
