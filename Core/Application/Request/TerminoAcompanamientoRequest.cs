namespace MiComanderaApp.Core.Application.Request
{
    public class TerminoRequest
    {
        public string Termino { get; set; } = string.Empty;
    }

    public class TerminoAssignRequest
    {
        public int TerminoId { get; set; }
        public int ProductoId { get; set; }
    }

    public class AcompanamientoAssignRequest
    {
        public int ProductId { get; set; }
        public int AccompanimentId { get; set; }
        public bool Required { get; set; }
        public int? MaximoSeleccion { get; set; }
    }

    public class AcompanamientoRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
