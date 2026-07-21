using Avalonia;
using Avalonia.Controls;
using MiComanderaApp.Core.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

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

        public Task<TResult?> ShowDialogAsync<TView, TViewModel, TResult>(PixelPoint? position = null)
            where TView : Window, new()
            where TViewModel : class, IDialogViewModel<TResult>
        {
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            return ShowDialogInternalAsync<TView, TViewModel, TResult>(viewModel, position);
        }

        public Task<TResult?> ShowDialogAsync<TView, TViewModel, TResult>(TViewModel viewModel, PixelPoint? position = null)
            where TView : Window, new()
            where TViewModel : class, IDialogViewModel<TResult>
        {
            return ShowDialogInternalAsync<TView, TViewModel, TResult>(viewModel, position);
        }

        private async Task<TResult?> ShowDialogInternalAsync<TView, TViewModel, TResult>(TViewModel viewModel, PixelPoint? position)
            where TView : Window, new()
            where TViewModel : class, IDialogViewModel<TResult>
        {
            var window = new TView
            {
                DataContext = viewModel
            };

            if (position.HasValue)
            {
                window.WindowStartupLocation = WindowStartupLocation.Manual;
                window.Position = position.Value;
            }

            viewModel.CloseRequested += result =>
            {
                window.Close(result);
            };

            return await window.ShowDialog<TResult?>(_windowProvider.MainWindow);
        }
    }
}