using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.ViewModels;

namespace MiComanderaApp.Presentation.Views.Dialogs.Modals;

public partial class CreateProductViewModel : ObservableObject, IDialogViewModel<ProductoRequest>
{

    public event Action<ProductoRequest?>? CloseRequested;

    [ObservableProperty] private string nombre = "";
    [ObservableProperty] private string codigo = "";
    [ObservableProperty] private decimal precio;
    [ObservableProperty] private string descripcion = "";
    [ObservableProperty] private bool activo = true;
    [ObservableProperty] private string categoriaSeleccionada = "";


    public ObservableCollection<string> Categorias { get; } =
        new()
        {
            "Comidas",
            "Bebidas",
            "Postres"
        };



    [RelayCommand]
    private void Guardar()
    {
        var producto = new ProductoRequest
        {
            Name = Nombre,
            CategoryName = "",
            Price = (double)Precio
        };

        CloseRequested?.Invoke(producto);
    }


    [RelayCommand]
    private void Cancelar()
    {
        CloseRequested?.Invoke(null);
    }

}
