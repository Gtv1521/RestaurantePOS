using System;
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData.Kernel;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Application.UseCases.Catalogo;
using MiComanderaApp.Core.Application.UseCases.Product;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using MiComanderaApp.ViewModels;

namespace MiComanderaApp.Presentation.Views.Dialogs.Modals;

public partial class NewUserModalViewModel : ViewModelBase
{
    private readonly INavigationService _navegation;
    private readonly GetAllCatalogoUseCase _getAllCatalogoUseCase;
    private readonly InsertProductUseCase _insertProductUseCase;


    public NewUserModalViewModel(
        INavigationService navegation,
        GetAllCatalogoUseCase getAllCatalogoUseCase,
        InsertProductUseCase insertProductUseCase

        )
    {
        _navegation = navegation;
        _insertProductUseCase = insertProductUseCase;
        _getAllCatalogoUseCase = getAllCatalogoUseCase;
        _ = LoadCategories();
    }

    [ObservableProperty] private string nombre = "";
    [ObservableProperty] private string codigo = "";
    [ObservableProperty] private decimal precio;
    [ObservableProperty] private string descripcion = "";
    [ObservableProperty] private bool activo = true;
    [ObservableProperty] private CatalogoModel? categoriaSeleccionada;
    [ObservableProperty] private string? _error;

    public ObservableCollection<CatalogoModel> Categorias { get; } = new();


    private async Task LoadCategories()
    {
        var categoriasData = await _getAllCatalogoUseCase.Execute();
        foreach (var item in categoriasData)
        {
            Categorias.Add(item);
        }
    }


    [RelayCommand]
    private async Task Guardar()
    {
        Error = "";
        if (Nombre == "" || Precio == 0 || CategoriaSeleccionada?.Id == 0) return;

        var producto = new ProductoRequest
        {
            Name = Nombre!,
            CategoryId = CategoriaSeleccionada?.Id ?? 0,
            Description = Descripcion,
            IsAvailable = Activo,
            Price = (double)Precio,
            ImageUrl = ""
        };

        try
        {
            await _insertProductUseCase.Execute(producto);
            _navegation.CloseOverlay();
        }
        catch (System.Exception ex)
        {
            Error = ex.Message;
            System.Console.WriteLine(ex.InnerException);
            System.Console.WriteLine(Error);
        }

    }


    [RelayCommand]
    private void Cancelar()
    {
        _navegation.CloseOverlay();
    }

}
