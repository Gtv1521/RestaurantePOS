using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.ViewModels;
using Microsoft.Extensions.Primitives;

namespace MiComanderaApp.Presentation.Views.Components.Generales;

public partial class TecladoComponentViewModel : ViewModelBase
{
    public event Action<string>? KeyPressed;


    [RelayCommand]
    private void InsertKey(string key)
    {
        KeyPressed?.Invoke(key);
        System.Console.WriteLine(key);
    }

    [RelayCommand]
    private void BackSpace()
    {
        System.Console.WriteLine("Borrar");
    }

    [RelayCommand]
    private void TabKey()
    {
        System.Console.WriteLine("Tabular");
    }
}
