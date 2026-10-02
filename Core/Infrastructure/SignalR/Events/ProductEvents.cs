using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace MiComanderaApp.Core.Infrastructure.SignalR.Events
{
    public class ProductEvents : ISignalREventHandler
    {
        public event Action<ProductoModel>? ProductCreated;
        public void Register(SignalRService signalR)
        {
            signalR.Connection.On<ProductoModel>("ProductCreated", (producto) =>
               {
                   ProductCreated?.Invoke(producto);
               });
        }
    }
}