using System;

namespace MiComanderaApp.Core.Domain.Models
{
    public class VentaActivaModel
    {
        public int VentaId { get; set; }
        public int NumeroMesa { get; set; }
        public decimal Total { get; set; }
        public DateTime FechaApertura { get; set; }
        public int ItemsCount { get; set; }
        public string Mesero { get; set; } = string.Empty;
        public string? Alias { get; set; }
        public int Instancia { get; set; }
        public int Pax { get; set; }

        public string HoraApertura => FechaApertura.ToLocalTime().ToString("HH:mm");
        public string MesaTexto => $"Mesa {NumeroMesa}";
        public string TotalTexto => $"S/ {Total:N2}";
    }
}
