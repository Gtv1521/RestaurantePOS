using System.Collections.Generic;

namespace MiComanderaApp.Core.Application.Request
{
    public class RecetaRequest
    {
        public int IngredientId { get; set; }
        public double QuantityNeeded { get; set; }
    }
}
