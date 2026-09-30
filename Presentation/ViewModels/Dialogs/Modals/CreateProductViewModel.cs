using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Application.UseCases.Catalogo;
using MiComanderaApp.Core.Application.UseCases.Product;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.Presentation.Views.Components.Generales;
using MiComanderaApp.ViewModels;

namespace MiComanderaApp.Presentation.Views.Dialogs.Modals;

public partial class CreateProductViewModel : ObservableValidator, IDialogViewModel<ProductoRequest>
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

    [ObservableProperty] private bool _mostrarErrores;
    [ObservableProperty] private bool _editar;
    [ObservableProperty] private string? _mensajeExito;
    [ObservableProperty]
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    private string? _nombre;
    [ObservableProperty] private bool _teclado = false;
    [ObservableProperty] private bool _botonTeclado = true;
    [ObservableProperty] private object? _vistaActual;
    [Required(ErrorMessage = "El código es obligatorio.")]
    [ObservableProperty]
    private string? _codigo;
    [ObservableProperty]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0.")]
    private decimal? _precio;
    [ObservableProperty]
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    private string? _descripcion;
    [ObservableProperty] private bool _activo = true;
    [ObservableProperty] private bool _loading = true;
    [ObservableProperty] private string _errorInsert = string.Empty;
    [ObservableProperty] private CatalogoModel _categoriaSeleccionada = new();

    public ObservableCollection<CatalogoModel> Categorias { get; } = new();
    public ObservableCollection<string> Errores { get; } = new();



    public void Initialize(ProductoModel? model)
    {
        Editar = true;
        if (model != null)
        {
            Nombre = model.Name;
            Codigo = model.Name;
            Precio = (decimal)model.Price;
            Descripcion = model.Description;
            Activo = model.IsAvailable;
            CategoriaSeleccionada = Categorias.FirstOrDefault(c => c.Id == model.Id) ?? new CatalogoModel();
        }
    }

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
            MostrarErrores = true;

            ValidateAllProperties();

            if (HasErrors)
                return;

            Loading = true;

            var producto = new ProductoRequest
            {
                Name = Nombre!,
                CategoryId = CategoriaSeleccionada.Id,
                Price = (double)Precio!,
                Description = Descripcion!,
                IsAvailable = Activo,
                ImageUrl = ""
            };

            var insertar = await _insertProductUseCase.Execute(producto);

            Console.WriteLine($"Producto insertado con ID: {insertar}");
            MensajeExito = "¡Producto agregado correctamente!";
            Loading = false; // opcional: quitar el spinner mientras se muestra el mensaje

            await Task.Delay(1000);
            MensajeExito = string.Empty;
            CloseRequested?.Invoke(null);

        }
        catch (System.Exception ex)
        {
            ErrorInsert = ex.Message;
            System.Console.WriteLine($"Error al guardar el producto: {ex.Message}");
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
