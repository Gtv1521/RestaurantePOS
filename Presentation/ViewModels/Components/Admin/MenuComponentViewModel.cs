using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Application.UseCases.Catalogo;
using MiComanderaApp.Core.Application.UseCases.Product;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.Presentation.Views.Dialogs.Modals;
using MiComanderaApp.ViewModels.Components.Products;

namespace MiComanderaApp.ViewModels.Components.Admin;

public partial class MenuComponentViewModel : ViewModelBase
{
    private readonly IViewModelFactory _factory;
    private readonly IDialogService _dialogService;
    private readonly GetAllProductUseCase _repo;
    private readonly GetAllCatalogoUseCase _categorias;
    private readonly GetCatalogoXIdProdUseCase _prodXIdCat;


    public ObservableCollection<ProductoModel> Products { get; } = new();
    public ObservableCollection<CatalogoModel> Categorias { get; } = new();

    public MenuComponentViewModel(
        IViewModelFactory factory,
        IDialogService dialogService,
        GetAllCatalogoUseCase categorias,
        GetCatalogoXIdProdUseCase oneCategoria,
        GetAllProductUseCase repo
        )
    {
        _factory = factory;
        _repo = repo;
        _prodXIdCat = oneCategoria;
        _dialogService = dialogService;
        _categorias = categorias;
        _ = LoadProducts();
        _ = CargeCategorias();
    }

    [RelayCommand]
    private async Task LoadProducts()
    {
        Products.Clear();
        var products = await _repo.Execute();


        foreach (var table in products)
        {
            Products.Add(table);
        }

    }

    [RelayCommand]
    private async Task CargeCategorias()
    {
        var categorias = await _categorias.Execute();
        foreach (var categoria in categorias)
        {
            Categorias.Add(categoria);
        }
    }

    [RelayCommand]
    private async Task ChangeCatalogo(int id)
    {
        var products = await _prodXIdCat.Execute(id);

        Products.Clear();
        foreach (var table in products)
        {
            Products.Add(table);
        }
    }

    [RelayCommand]
    private async Task NuevoProducto()
    {
        var producto =
        await _dialogService
        .ShowDialogAsync<CreateProduct, CreateProductViewModel, ProductoRequest>(
            new PixelPoint(250, 30)
            );

        if (producto != null)
        {

            System.Console.WriteLine(producto);
        }
    }

}
