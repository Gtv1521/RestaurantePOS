using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace MiComanderaApp.Core.Infrastructure.SignalR.Events
{
    /// <summary>
    /// Recibe los cambios de disponibilidad de productos enviados por el servidor.
    /// El backend emite "ReceiveProductAvailabilityUpdate" al descontar stock de una
    /// comanda y "ReceiveProductUpdate" cuando se edita/desactiva un producto.
    /// </summary>
    public class AvailabilityEvents : ISignalREventHandler
    {
        public event Action<ProductoModel>? AvailabilityChanged;

        public void Register(SignalRService signalR)
        {
            signalR.Connection.On<ProductoModel>("ReceiveProductAvailabilityUpdate", producto =>
            {
                AvailabilityChanged?.Invoke(producto);
            });

            signalR.Connection.On<ProductoModel>("ReceiveProductUpdate", producto =>
            {
                AvailabilityChanged?.Invoke(producto);
            });
        }
    }
}
