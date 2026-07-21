using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;

namespace MiComanderaApp.Core.Domain.Interfaces
{
    public interface IDialogService
    {
        Task<TResult?> ShowDialogAsync<TView, TViewModel, TResult>(PixelPoint? position = null)
        where TView : Window, new()
        where TViewModel : class, IDialogViewModel<TResult>;
    }
}