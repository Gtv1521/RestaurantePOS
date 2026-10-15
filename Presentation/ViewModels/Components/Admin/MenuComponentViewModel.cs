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
using MiComanderaApp.Core.Application.UseCases.Acompanamiento;
using MiComanderaApp.Core.Application.UseCases.Catalogo;
using MiComanderaApp.Core.Application.UseCases.Observacion;
using MiComanderaApp.Core.Application.UseCases.Product;
using MiComanderaApp.Core.Application.UseCases.Termino;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Core.Infrastructure.SignalR.Events;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.Presentation.Services;
using MiComanderaApp.Presentation.Views.Dialogs.Modals;
using MiComanderaApp.ViewModels.Components.Products;
using MiComanderaApp.ViewModels.Dialogs.Modals;
using MiComanderaApp.Views.Dialogs.Modals;
using Microsoft.Extensions.DependencyInjection;

namespace MiComanderaApp.ViewModels.Components.Admin;

public partial class MenuComponentViewModel : ViewModelBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDialogService _dialogService;
    private readonly GetAllProductUseCase _repo;
    private readonly GetAllCatalogoUseCase _categorias;
    private readonly GetCatalogoXIdProdUseCase _prodXIdCat;
    private readonly ProductEvents _productEvents;
    private readonly DeleteProductUseCase _deleteProductUseCase;
    private readonly GetAllTerminosUseCase _todosTerminos;
    private readonly CreateTerminoUseCase _createTermino;
    private readonly DeleteTerminoUseCase _deleteTermino;
    private readonly GetAllAcompanamientosUseCase _todosAcomp;
    private readonly CreateAcompanamientoUseCase _createAcomp;
    private readonly DeleteAcompanamientoUseCase _deleteAcomp;
    private readonly CreateCatalogoUseCase _createCatalogo;
    private readonly DeleteCatalogoUseCase _deleteCatalogo;
    private readonly GetAllObservacionUseCase _todosObservaciones;
    private readonly CreateObservacionUseCase _createObservacion;
    private readonly DeleteObservacionUseCase _deleteObservacion;
    private readonly IViewModelFactory _factory;
    public LoadingState Loading { get; }


    public ObservableCollection<ProductoModel> Products { get; } = new();
    public ObservableCollection<CatalogoModel> Categorias { get; } = new();

    public MenuComponentViewModel(
        IServiceProvider serviceProvider,
        IDialogService dialogService,
        GetAllCatalogoUseCase categorias,
        DeleteProductUseCase deleteProductUseCase,
        GetCatalogoXIdProdUseCase oneCategoria,
        ProductEvents productEvents,
        GetAllProductUseCase repo,
        GetAllTerminosUseCase todosTerminos,
        CreateTerminoUseCase createTermino,
        DeleteTerminoUseCase deleteTermino,
        GetAllAcompanamientosUseCase todosAcomp,
        CreateAcompanamientoUseCase createAcomp,
        DeleteAcompanamientoUseCase deleteAcomp,
        CreateCatalogoUseCase createCatalogo,
        DeleteCatalogoUseCase deleteCatalogo,
        GetAllObservacionUseCase todosObservaciones,
        CreateObservacionUseCase createObservacion,
        DeleteObservacionUseCase deleteObservacion,
        IViewModelFactory factory,
        LoadingState loading
        )
    {
        _serviceProvider = serviceProvider;
        _repo = repo;
        _prodXIdCat = oneCategoria;
        _dialogService = dialogService;
        _categorias = categorias;
        _todosTerminos = todosTerminos;
        _createTermino = createTermino;
        _deleteTermino = deleteTermino;
        _todosAcomp = todosAcomp;
        _createAcomp = createAcomp;
        _deleteAcomp = deleteAcomp;
        _createCatalogo = createCatalogo;
        _deleteCatalogo = deleteCatalogo;
        _todosObservaciones = todosObservaciones;
        _createObservacion = createObservacion;
        _deleteObservacion = deleteObservacion;
        _deleteProductUseCase = deleteProductUseCase;
        _productEvents = productEvents;
        Loading = loading;
        _ = LoadProducts();
        _ = CargeCategorias();
        _productEvents.ProductCreated += OnProductCreated;
        _factory = factory;
    }

    private void OnProductCreated(ProductoModel producto)
    {
        Products.Add(producto);
    }

    [RelayCommand]
    private async Task LoadProducts()
    {
        try
        {
            Loading.Show("Cargando menú...");
            Products.Clear();
            var products = await _repo.Execute();


            foreach (var table in products)
            {
                Products.Add(table);
            }
        }
        finally
        {
            Loading.Hide();
        }

    }

    [RelayCommand]
    private async Task CargeCategorias()
    {
        try
        {
            Loading.Show("Cargando menú...");
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
            Loading.Hide();
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
    private async Task EditProduct(ProductoModel? producto)
    {
        if (producto == null) return;

        var viewModel = _serviceProvider.GetRequiredService<CreateProductViewModel>();
        viewModel.Initialize(producto);

        var result = await _dialogService
            .ShowDialogAsync<CreateProduct, CreateProductViewModel, ProductoRequest>(
                viewModel,
                new PixelPoint(250, 30));

        if (result != null)
        {
            await LoadProducts();
        }
    }

    [RelayCommand]
    private async Task GestionarCatalogoTerminos()
    {
        var viewModel = new TerminosCatalogoViewModel(
            _todosTerminos,
            _createTermino,
            _deleteTermino,
            _factory);

        await _dialogService
            .ShowDialogAsync<TerminosCatalogoView, TerminosCatalogoViewModel, bool?>(
                viewModel,
                new PixelPoint(250, 30));
    }

    [RelayCommand]
    private async Task GestionarCatalogoCategorias()
    {
        var viewModel = new CategoriasCatalogoViewModel(
            _categorias,
            _createCatalogo,
            _deleteCatalogo,
            _factory);

        var changed = await _dialogService
            .ShowDialogAsync<CategoriasCatalogoView, CategoriasCatalogoViewModel, bool?>(
                viewModel,
                new PixelPoint(250, 30));

        if (changed == true)
        {
            Categorias.Clear();
            await CargeCategorias();
        }
    }

    [RelayCommand]
    private async Task GestionarCatalogoObservaciones()
    {
        var viewModel = new ObservacionesCatalogoViewModel(
            _todosObservaciones,
            _createObservacion,
            _deleteObservacion,
            _factory);

        await _dialogService
            .ShowDialogAsync<ObservacionesCatalogoView, ObservacionesCatalogoViewModel, bool?>(
                viewModel,
                new PixelPoint(250, 30));
    }

    [RelayCommand]
    private async Task GestionarCatalogoAcompanamientos()
    {
        var viewModel = new AcompanamientosCatalogoViewModel(
            _todosAcomp,
            _createAcomp,
            _deleteAcomp,
            _factory);

        await _dialogService
            .ShowDialogAsync<AcompanamientosCatalogoView, AcompanamientosCatalogoViewModel, bool?>(
                viewModel,
                new PixelPoint(250, 30));
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
