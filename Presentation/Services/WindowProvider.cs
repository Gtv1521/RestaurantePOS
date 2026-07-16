using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using MiComanderaApp.Core.Domain.Interfaces;

namespace MiComanderaApp.Presentation.Services
{
    public class WindowProvider : IWindowProvider
    {
        public Window MainWindow { get; set; } = null!;
    }
}