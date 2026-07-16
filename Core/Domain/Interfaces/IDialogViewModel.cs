using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiComanderaApp.Core.Domain.Interfaces
{
    public interface IDialogViewModel<TResult>
    {
        event Action<TResult?>? CloseRequested;
    }
}