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
        public List<int> CategoryIds { get; set; } = new();

    public string NombreConCategoria { get; set; } = "";

        public string ConnectionTypeTexto => ConnectionType == 0 ? "Red" : "USB";
        public string NombreCompleto => $"{Name} ({ConnectionTypeTexto})";
    }
}
