using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Infrastructure.SignalR;

namespace MiComanderaApp.Core.Domain.Interfaces
{
    public interface ISignalREventHandler
    {
        void Register(SignalRService signalR);
    }
}