using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.ViewModels;

namespace MiComanderaApp.Interfaces
{
    public interface IViewModelFactory
    {
        T Create<T>() where T : ViewModelBase;
    }
}