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

        public Task<TResult?> ShowDialogAsync<TViewModel, TResult>(TViewModel viewModel, PixelPoint? position = null)
            where TViewModel : class, IDialogViewModel<TResult>
        {
            return ShowDialogInternalByViewModelAsync<TViewModel, TResult>(viewModel, position);
        }

        public Task<TResult?> ShowDialogAsync<TViewModel, TResult>(PixelPoint? position = null)
            where TViewModel : class, IDialogViewModel<TResult>
        {
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            return ShowDialogInternalByViewModelAsync<TViewModel, TResult>(viewModel, position);
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

        private async Task<TResult?> ShowDialogInternalByViewModelAsync<TViewModel, TResult>(TViewModel viewModel, PixelPoint? position)
            where TViewModel : class, IDialogViewModel<TResult>
        {
            // Try to find corresponding view by convention: replace .ViewModels. with .Views. and ViewModel suffix with View
            var vmType = typeof(TViewModel);
            var vmFullName = vmType.FullName ?? string.Empty;
            var viewFullName = vmFullName.Replace(".ViewModels.", ".Views.").Replace("ViewModel", "View");

            Type? viewType = null;
            // Search in loaded assemblies
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                viewType = asm.GetType(viewFullName);
                if (viewType != null) break;
            }

            if (viewType == null)
                throw new InvalidOperationException($"View type '{viewFullName}' not found for ViewModel '{vmFullName}'");

            var control = Activator.CreateInstance(viewType) as Control;
            if (control == null)
                throw new InvalidOperationException($"View '{viewFullName}' is not a Control or could not be instantiated.");

            var window = new Window
            {
                Content = control
            };

            // Make the window chrome-less and transparent like other modal windows in the app
            window.SystemDecorations = SystemDecorations.None;
            window.Background = Avalonia.Media.Brushes.Transparent;
            window.ShowInTaskbar = false;
            window.SizeToContent = SizeToContent.WidthAndHeight;

            control.DataContext = viewModel;

            if (position.HasValue)
            {
                window.WindowStartupLocation = WindowStartupLocation.Manual;
                window.Position = position.Value;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            viewModel.CloseRequested += result => window.Close(result);

            return await window.ShowDialog<TResult?>(_windowProvider.MainWindow);
        }
    }
}