using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Application.UseCases.Catalogo;
using MiComanderaApp.Core.Application.UseCases.Product;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Presentation.Views.Components.Generales;
using MiComanderaApp.ViewModels;

namespace MiComanderaApp.Presentation.Views.Dialogs.Modals;

public partial class CreateProductViewModel : ObservableObject, IDialogViewModel<ProductoRequest>
{
    private readonly GetAllCatalogoUseCase _allCatalogoCase;
    private readonly InsertProductUseCase _insertProductUseCase;
    private readonly IViewModelFactory _factory;


    public CreateProductViewModel(
        GetAllCatalogoUseCase allCatalogoCase, 
        InsertProductUseCase insertProductUseCase,
        IViewModelFactory factory
        )
    {
        _allCatalogoCase = allCatalogoCase;
        _insertProductUseCase = insertProductUseCase;
        _factory = factory;
        _ = LoadCategories();
        _vistaActual = _factory.Create<TecladoComponentViewModel>();

    }


    public event Action<ProductoRequest?>? CloseRequested;

    [ObservableProperty] private string _nombre = "";
    [ObservableProperty] private bool _teclado = false;
    [ObservableProperty] private bool _botonTeclado = true;
     [ObservableProperty] private object? _vistaActual;
    [ObservableProperty] private string _codigo = "";
    [ObservableProperty] private decimal _precio;
    [ObservableProperty] private string _descripcion = "";
    [ObservableProperty] private bool _activo = true;
    [ObservableProperty] private bool _loading = true;
    [ObservableProperty] private CatalogoModel _categoriaSeleccionada = new();
    public ObservableCollection<CatalogoModel> Categorias { get; } = new();


    private async Task LoadCategories()
    {
        var categorias = await _allCatalogoCase.Execute();
        foreach (var item in categorias)
        {
            Categorias.Add(item);
        }
    }


    [RelayCommand]
    private async Task GuardarAsync()
    {
        try
        {
            Loading = true;

            var producto = new ProductoRequest
            {
                Name = Nombre,
                CategoryId = CategoriaSeleccionada.Id,
                Price = (double)Precio,
                Description = Descripcion,
                IsAvailable = Activo,
                ImageUrl = ""
            };

            var insertar = await _insertProductUseCase.Execute(producto);
            CloseRequested?.Invoke(producto);

        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine(ex.Message);
        }
        finally
        {
            Loading = false;
        }
    }

    [RelayCommand]
    private void VerTeclado()
    {
        Teclado = !Teclado;
        BotonTeclado = !BotonTeclado;
    }

    [RelayCommand]
    private void Cancelar()
    {
        CloseRequested?.Invoke(null);
    }

}
