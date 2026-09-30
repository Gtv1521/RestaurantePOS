using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Application.UseCases.Catalogo;
using MiComanderaApp.Core.Application.UseCases.Product;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Core.Infrastructure.SignalR.Events;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.Presentation.Views.Dialogs.Modals;
using MiComanderaApp.ViewModels.Components.Products;

namespace MiComanderaApp.ViewModels.Components.Admin;

public partial class MenuComponentViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly GetAllProductUseCase _repo;
    private readonly GetAllCatalogoUseCase _categorias;
    private readonly GetCatalogoXIdProdUseCase _prodXIdCat;
    private readonly ProductEvents _productEvents;
    private readonly DeleteProductUseCase _deleteProductUseCase;


    public ObservableCollection<ProductoModel> Products { get; } = new();
    public ObservableCollection<CatalogoModel> Categorias { get; } = new();

    [ObservableProperty] private bool _isLoading = false;
    public MenuComponentViewModel(
        IDialogService dialogService,
        GetAllCatalogoUseCase categorias,
        DeleteProductUseCase deleteProductUseCase,
        GetCatalogoXIdProdUseCase oneCategoria,
        ProductEvents productEvents,
        GetAllProductUseCase repo
        )
    {
        _repo = repo;
        _prodXIdCat = oneCategoria;
        _dialogService = dialogService;
        _categorias = categorias;
        _deleteProductUseCase = deleteProductUseCase;
        _productEvents = productEvents;
        _ = LoadProducts();
        _ = CargeCategorias();
        _productEvents.ProductCreated += OnProductCreated;
    }

    private void OnProductCreated(ProductoModel producto)
    {
        Products.Add(producto);
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
        try
        {
            IsLoading = true;
            var categorias = await _categorias.Execute();
            foreach (var categoria in categorias)
            {
                Categorias.Add(categoria);
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine(ex.Message);
            throw;
        }
        finally
        {
            IsLoading = false;
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
    private async Task UpdateProducto(ProductoModel producto)
    {

        var updatedProduct =
            await _dialogService
            .ShowDialogAsync<CreateProduct, CreateProductViewModel, ProductoRequest>(
                new PixelPoint(250, 30)
                );


        if (updatedProduct != null)
        {
            System.Console.WriteLine(updatedProduct);
            await LoadProducts();
        }

    }

    [RelayCommand]
    private async Task DeleteProduct(ProductoModel producto)
    {
        try
        {
            System.Console.WriteLine($"Eliminando producto con ID: {producto.Id}");
            var result = await _deleteProductUseCase.ExecuteAsync(producto.Id);
            Products.Remove(producto);
        }
        catch (System.Exception e)
        {
            System.Console.WriteLine(e.Message);
            throw;
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
