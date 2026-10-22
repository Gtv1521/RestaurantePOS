namespace MiComanderaApp.Core.Domain.Models
{
    public class ImpresoraDescubiertaModel
    {
        public string Name { get; set; } = string.Empty;
        public int ConnectionType { get; set; }
        public string Address { get; set; } = string.Empty;
        public int Port { get; set; }
        public string DiscoveryMethod { get; set; } = string.Empty;
        public bool IsRegistered { get; set; }
        public int? RegisteredPrinterId { get; set; }

        public string ConnectionTypeTexto => ConnectionType == 0 ? "Red" : "USB";
        public string EstadoTexto => IsRegistered ? "Ya registrada" : "Nueva";
        public string DireccionCompleta => ConnectionType == 1 ? Address : $"{Address}:{Port}";
    }
}
