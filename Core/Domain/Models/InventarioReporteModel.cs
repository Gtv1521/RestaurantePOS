using System;
using System.Collections.Generic;

namespace MiComanderaApp.Core.Domain.Models
{
    public class InventarioReporteModel
    {
        public ResumenInventarioModel Resumen { get; set; } = new();
        public List<IngredienteCriticoModel> IngredientesCriticos { get; set; } = new();
        public List<CostoProductoModel> CostosProductos { get; set; } = new();
        public List<MovimientoInventarioModel> MovimientosRecientes { get; set; } = new();
    }

    public class ResumenInventarioModel
    {
        public int TotalIngredientes { get; set; }
        public int IngredientesBajoStock { get; set; }
        public int IngredientesCriticos { get; set; }
        public decimal ValorTotalInventario { get; set; }
        public decimal CostoPromedioProducto { get; set; }
        public decimal MargenGananciaPromedio { get; set; }
    }

    public class IngredienteCriticoModel
    {
        public int IngredienteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty;
        public decimal CantidadDisponible { get; set; }
        public decimal CantidadMinima { get; set; }
        public string Nivel { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }

    public class CostoProductoModel
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioVenta { get; set; }
        public decimal CostoProduccion { get; set; }
        public decimal Ganancia { get; set; }
        public double MargenGanancia { get; set; }
        public int UnidadesVendidas { get; set; }
        public decimal GananciaTotal { get; set; }
    }

    public class MovimientoInventarioModel
    {
        public int Id { get; set; }
        public string Ingrediente { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
    }
}
