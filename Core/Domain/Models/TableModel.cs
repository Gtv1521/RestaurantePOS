using System;

namespace MiComanderaApp.Models;

public class TableModel
{
    public int Id { get; set; }
    public int NumeroMesa { get; set; }
    public int Capacidad { get; set; }
    public string Estado { get; set; } = "";
    public string Ubicacion { get; set; } = "";
    public bool Ocupada { get; set; }
    public VentaModel? VentasActivas { get; set; }
}

public class VentaModel
{
    public int VentaId { get; set; }
    public int NumeroMesa { get; set; }
    public int Total { get; set; }
    public DateTime FechaApertura { get; set; }
    public int ItemsCount { get; set; }
    public string Mesero { get; set; } = "";
    public string Alias { get; set; } = "";
    public int Instancia { get; set; }
}