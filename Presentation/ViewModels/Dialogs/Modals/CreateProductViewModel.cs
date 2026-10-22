using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Application.UseCases.Catalogo;
using MiComanderaApp.Core.Application.UseCases.Print;
using MiComanderaApp.Core.Application.UseCases.Product;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.Presentation.Views.Components.Generales;
using MiComanderaApp.ViewModels;

namespace MiComanderaApp.Presentation.Views.Dialogs.Modals;

/// <summary>Impresora disponible con su estado de selección en el modal de producto.</summary>
public partial class ImpresoraSeleccionItem : ObservableObject
{
    public int Id { get; }
    public string Name { get; }

    [ObservableProperty] private bool _isSelected;

    public ImpresoraSeleccionItem(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

public partial class CreateProductViewModel : ObservableValidator, IDialogViewModel<ProductoRequest>
{
    private readonly GetAllCatalogoUseCase _allCatalogoCase;
    private readonly InsertProductUseCase _insertProductUseCase;
    private readonly UpdateProductUseCase _updateProductUseCase;
    private readonly GetImpresorasUseCase _getImpresorasUseCase;
    private readonly IViewModelFactory _factory;

    private List<int> _printerIdsPendientes = new();
    private List<int> _acompanamientoPrinterIdsPendientes = new();


    public CreateProductViewModel(
        GetAllCatalogoUseCase allCatalogoCase,
        InsertProductUseCase insertProductUseCase,
        UpdateProductUseCase updateProductUseCase,
        GetImpresorasUseCase getImpresorasUseCase,
        IViewModelFactory factory
        )
    {
        _allCatalogoCase = allCatalogoCase;
        _insertProductUseCase = insertProductUseCase;
        _updateProductUseCase = updateProductUseCase;
        _getImpresorasUseCase = getImpresorasUseCase;
        _factory = factory;
        _ = LoadCategories();
        _ = LoadImpresoras();
        _vistaActual = _factory.Create<TecladoComponentViewModel>();

    }


    public event Action<ProductoRequest?>? CloseRequested;

    [ObservableProperty] private bool _mostrarErrores;
    [ObservableProperty] private bool _editar;
    [ObservableProperty] private string? _mensajeExito;
    [ObservableProperty] private string _titulo = "Crear Producto";
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
    [ObservableProperty] private bool _conTermino;
    [ObservableProperty] private bool _llevaAcompanamientos;
    [ObservableProperty] private int _maxAcompanamientos = 2;
    [ObservableProperty] private int _productoId;
    [ObservableProperty] private bool _loading = true;
    [ObservableProperty] private string _errorInsert = string.Empty;
    [ObservableProperty] private CatalogoModel _categoriaSeleccionada = new();

    public ObservableCollection<CatalogoModel> Categorias { get; } = new();
    public ObservableCollection<string> Errores { get; } = new();

    /// <summary>Impresoras que imprimen el plato completo.</summary>
    public ObservableCollection<ImpresoraSeleccionItem> ImpresorasPlato { get; } = new();

    /// <summary>Impresoras que imprimen solo los acompañamientos.</summary>
    public ObservableCollection<ImpresoraSeleccionItem> ImpresorasAcompanamientos { get; } = new();

    public bool HayImpresoras => ImpresorasPlato.Count > 0;



    public void Initialize(ProductoModel? model)
    {
        Editar = true;
        Titulo = "Editar Producto";
        if (model != null)
        {
            ProductoId = model.Id;
            Nombre = model.Name;
            Codigo = model.Name;
            Precio = (decimal)model.Price;
            Descripcion = model.Description;
            Activo = model.IsAvailable;
            ConTermino = model.TieneTerminos;
            LlevaAcompanamientos = model.TieneAcompanamientos;
            MaxAcompanamientos = model.MaxAccompaniments > 0 ? model.MaxAccompaniments : 2;
            CategoriaSeleccionada = Categorias.FirstOrDefault(c =>
                string.Equals(c.Name, model.CategoryName, StringComparison.OrdinalIgnoreCase))
                ?? new CatalogoModel();

            _printerIdsPendientes = model.PrinterIds.Distinct().ToList();
            _acompanamientoPrinterIdsPendientes = model.AccompanimentPrinterIds.Distinct().ToList();
            AplicarSeleccionImpresoras();
        }
    }

    private void AplicarSeleccionImpresoras()
    {
        foreach (var item in ImpresorasPlato)
            item.IsSelected = _printerIdsPendientes.Contains(item.Id);

        foreach (var item in ImpresorasAcompanamientos)
            item.IsSelected = _acompanamientoPrinterIdsPendientes.Contains(item.Id);
    }

    private async Task LoadImpresoras()
    {
        try
        {
            var impresoras = await _getImpresorasUseCase.Execute();
            ImpresorasPlato.Clear();
            ImpresorasAcompanamientos.Clear();
            foreach (var impresora in impresoras.Where(i => i.Active))
            {
                ImpresorasPlato.Add(new ImpresoraSeleccionItem(impresora.Id, impresora.Name));
                ImpresorasAcompanamientos.Add(new ImpresoraSeleccionItem(impresora.Id, impresora.Name));
            }
            OnPropertyChanged(nameof(HayImpresoras));
            AplicarSeleccionImpresoras();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar las impresoras: {ex.Message}");
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
                Id = ProductoId,
                Name = Nombre!,
                CategoryId = CategoriaSeleccionada.Id,
                Price = (double)Precio!,
                Description = Descripcion!,
                IsAvailable = Activo,
                ImageUrl = "",
                ConTermino = ConTermino,
                HasAccompaniments = LlevaAcompanamientos,
                MaxAccompaniments = MaxAcompanamientos,
                PrinterIds = ImpresorasPlato.Where(i => i.IsSelected).Select(i => i.Id).ToList(),
                AccompanimentPrinterIds = ImpresorasAcompanamientos.Where(i => i.IsSelected).Select(i => i.Id).ToList()
            };

            if (Editar)
            {
                var actualizado = await _updateProductUseCase.Execute(ProductoId, producto);

                if (!actualizado)
                {
                    Errores.Clear();
                    Errores.Add("Error al actualizar el producto.");
                    return;
                }

                CloseRequested?.Invoke(producto);
                return;
            }

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
