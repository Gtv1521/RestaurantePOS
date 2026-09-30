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
using MiComanderaApp.Core.Application.UseCases.Product;
using MiComanderaApp.Core.Application.UseCases.Termino;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.Presentation.Views.Dialogs.Modals;
using MiComanderaApp.ViewModels.Components.Products;
using MiComanderaApp.ViewModels.Dialogs.Modals;
using MiComanderaApp.Views.Dialogs.Modals;
using Microsoft.Extensions.DependencyInjection;

namespace MiComanderaApp.ViewModels.Components.Admin;

public partial class MenuComponentViewModel : ViewModelBase
{
    private readonly IViewModelFactory _factory;
    private readonly IServiceProvider _serviceProvider;
    private readonly IDialogService _dialogService;
    private readonly GetAllProductUseCase _repo;
    private readonly GetAllCatalogoUseCase _categorias;
    private readonly GetCatalogoXIdProdUseCase _prodXIdCat;
    private readonly GetAllTerminosUseCase _todosTerminos;
    private readonly CreateTerminoUseCase _createTermino;
    private readonly DeleteTerminoUseCase _deleteTermino;
    private readonly GetAllAcompanamientosUseCase _todosAcomp;
    private readonly CreateAcompanamientoUseCase _createAcomp;
    private readonly DeleteAcompanamientoUseCase _deleteAcomp;


    public ObservableCollection<ProductoModel> Products { get; } = new();
    public ObservableCollection<CatalogoModel> Categorias { get; } = new();

    [ObservableProperty] private bool _isLoading = false;
    public MenuComponentViewModel(
        IViewModelFactory factory,
        IServiceProvider serviceProvider,
        IDialogService dialogService,
        GetAllCatalogoUseCase categorias,
        GetCatalogoXIdProdUseCase oneCategoria,
        GetAllProductUseCase repo,
        GetAllTerminosUseCase todosTerminos,
        CreateTerminoUseCase createTermino,
        DeleteTerminoUseCase deleteTermino,
        GetAllAcompanamientosUseCase todosAcomp,
        CreateAcompanamientoUseCase createAcomp,
        DeleteAcompanamientoUseCase deleteAcomp
        )
    {
        _factory = factory;
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
    private async Task EditarProducto(ProductoModel? producto)
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
