using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Interfaces;
using MiComanderaApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MiComanderaApp.Services.Factorys
{
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewModelFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public T Create<T>() where T : ViewModelBase
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}