using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace MiComanderaApp.Core.Infrastructure.SignalR.Events
{
    public class OrderEvents : ISignalREventHandler
    {
        public event Action<string>? OrderCreated;

        public void Register(SignalRService signalR)
        {
            signalR.Connection.On<string>("Notification", message =>
            {
                Console.WriteLine(message);
                OrderCreated?.Invoke(message);
            });
        }
    }
}