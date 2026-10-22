using System.Collections.Generic;

namespace MiComanderaApp.Core.Domain.Models
{
    public class ImpresoraModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ConnectionType { get; set; }
        public string Address { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool Active { get; set; }
        public int? BackupPrinterId { get; set; }
        public string? BackupPrinterName { get; set; }

        public string ConnectionTypeTexto => ConnectionType == 0 ? "Red" : "USB";
        public string NombreCompleto => $"{Name} ({ConnectionTypeTexto})";
    }
}
