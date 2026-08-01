using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Models;

namespace MiComanderaApp.Presentation.Messages
{
    public class TableOpenedMessage
    {
        public int TableNumber { get; }
        public List<VentaModel> Ventas { get; }

        public TableOpenedMessage(int tableNumber, List<VentaModel> ventas)
        {
            TableNumber = tableNumber;
            Ventas = ventas;
        }
    }
}