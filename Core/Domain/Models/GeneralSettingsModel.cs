using System;

namespace MiComanderaApp.Core.Domain.Models
{
    public class GeneralSettingsModel
    {
        public int Id { get; set; }

        // ===== Datos del negocio =====
        public string BusinessName { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }

        // ===== Moneda =====
        public string CurrencySymbol { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
        public int DecimalPlaces { get; set; } = 2;

        // ===== Impuesto al consumo =====
        public bool TaxEnabled { get; set; }
        public decimal TaxPercentage { get; set; }
        public bool TaxIncludedInPrice { get; set; }
        public string TaxLabel { get; set; } = string.Empty;

        // ===== Servicio / propina =====
        public bool ServiceChargeEnabled { get; set; }
        public decimal ServiceChargePercentage { get; set; }
        public string ServiceChargeLabel { get; set; } = string.Empty;

        // ===== Ticket =====
        public int TicketWidthChars { get; set; }
        public string TicketFooter { get; set; } = string.Empty;

        // ===== Operación =====
        public int DefaultPax { get; set; }
        public int DefaultPrinterPort { get; set; }

        /// <summary>Impresora de caja para facturas y comprobantes. Null/0 = preguntar.</summary>
        public int? DefaultInvoicePrinterId { get; set; }

        // ===== Auditoría =====
        public DateTime UpdatedAt { get; set; }
    }
}
