using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace MiComanderaApp.Core.Infrastructure.SignalR.Events
{
    public class TablesEvents : ISignalREventHandler
    {
        public event Action<string, string>? OpenTable;

        public void Register(SignalRService signalR)
        {
            signalR.Connection.On<string, string>("SendMessage", (title, message) =>
            {
                Console.WriteLine(message);
                OpenTable?.Invoke(title, message);
            });
        }
    }
}