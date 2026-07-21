using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveChartsCore;

namespace MiComanderaApp.Models
{
    public class SalesCardModel
    {
        public string Title { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public string PercentageText { get; set; } = string.Empty;

        // 🚀 Cada tarjeta guarda su propio arreglo de series para la gráfica
        public ISeries[] ChartSeries { get; set; } = Array.Empty<ISeries>();
    }
}