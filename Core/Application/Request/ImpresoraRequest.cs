using System.Collections.Generic;

namespace MiComanderaApp.Core.Application.Request
{
    public class ImpresoraRequest
    {
        public string Name { get; set; } = string.Empty;
        public int ConnectionType { get; set; }
        public string Address { get; set; } = string.Empty;
        public int Port { get; set; } = 9100;
        public bool Active { get; set; } = true;
        public int? BackupPrinterId { get; set; }
    }
}
