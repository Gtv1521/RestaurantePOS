namespace MiComanderaApp.Models;

public enum TableStatus { Disponible, Ocupada, Reservada }

public class TableModel
{
    public int TableNumber { get; set; }
    public TableStatus Status { get; set; }
}