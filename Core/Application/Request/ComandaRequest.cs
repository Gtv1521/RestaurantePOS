using System.Collections.Generic;

namespace MiComanderaApp.Core.Application.Request
{
    public class ComandaItemRequest
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public int? TerminoId { get; set; }
        public List<int> AcompanamientoIds { get; set; } = new();
    }

    public class ComandaCreateRequest
    {
        public int VentaId { get; set; }
        public int MeseroId { get; set; }
        public List<ComandaItemRequest> Items { get; set; } = new();
    }
}
