using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using MiComanderaApp.Core.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MiComanderaApp.Presentation.Services
{
    public class DialogService : IDialogService
    {
        private readonly IWindowProvider _windowProvider;
        private readonly IServiceProvider _serviceProvider;

        public DialogService(IWindowProvider windowProvider, IServiceProvider serviceProvider)
        {
            _windowProvider = windowProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<TResult?> ShowDialogAsync<TView, TViewModel, TResult>(PixelPoint? posicion)
     where TView : Window, new()
     where TViewModel : class, IDialogViewModel<TResult>
        {
            var window = new TView();

            var viewModel = _serviceProvider
                .GetRequiredService<TViewModel>();

            window.DataContext = viewModel;

            if (posicion.HasValue)
            {
                window.WindowStartupLocation = WindowStartupLocation.Manual;
                window.Position = posicion.Value;
            }

            viewModel.CloseRequested += result =>
            {
                window.Close(result);
            };

            return await window.ShowDialog<TResult>(_windowProvider.MainWindow);
        }
    }
}