using Avalonia;
using Avalonia.Controls;
using System.Threading.Tasks;

namespace MiComanderaApp.Core.Domain.Interfaces
{
    public interface IDialogService
    {
        Task<TResult?> ShowDialogAsync<TView, TViewModel, TResult>(PixelPoint? position = null)
            where TView : Window, new()
            where TViewModel : class, IDialogViewModel<TResult>;

        Task<TResult?> ShowDialogAsync<TView, TViewModel, TResult>(TViewModel viewModel, PixelPoint? position = null)
            where TView : Window, new()
            where TViewModel : class, IDialogViewModel<TResult>;
    }
}