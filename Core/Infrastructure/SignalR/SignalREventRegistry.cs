using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Domain.Interfaces;

namespace MiComanderaApp.Core.Infrastructure.SignalR
{
    public class SignalREventRegistry
    {
        private readonly IEnumerable<ISignalREventHandler> _handlers;


        public SignalREventRegistry(
            IEnumerable<ISignalREventHandler> handlers)
        {
            _handlers = handlers;
        }


        public void RegisterAll(SignalRService signalR)
        {
            foreach (var handler in _handlers)
            {
                handler.Register(signalR);
            }
        }

    }
}