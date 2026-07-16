using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace MiComanderaApp.Core.Domain.Interfaces
{
    public interface IWindowProvider
    {
        Window MainWindow { get; set; }
    }
}