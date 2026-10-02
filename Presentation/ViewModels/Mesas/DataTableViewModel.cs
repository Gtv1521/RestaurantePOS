using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Application.UseCases.Acompanamiento;
using MiComanderaApp.Core.Application.UseCases.Catalogo;
using MiComanderaApp.Core.Application.UseCases.Observacion;
using MiComanderaApp.Core.Application.UseCases.Session;
using MiComanderaApp.Core.Application.UseCases.Termino;
using MiComanderaApp.Core.Application.UseCases.Venta;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.ViewModels.Orders;

namespace MiComanderaApp.ViewModels.Mesas;


public class ProductoItem
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public bool TieneTerminos { get; set; }
    public bool TieneAcompanamientos { get; set; }
    public int MaxAccompaniments { get; set; } = 2;
}

public class CategoriaItem
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
public partial class ObservacionItem : ObservableObject
{
    public int Id { get; set; }
    public string Item { get; set; } = "";
    public decimal? Precio { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Valor))]
    private int? _cantidad;
    public decimal? Valor => Cantidad * Precio;
    public bool TienePrecio => Precio.HasValue;
    public bool TieneCantidad => Cantidad.HasValue;
}

public partial class ProductoPedidoItem : ObservableObject
{
    public int Indice { get; set; }
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public int? TerminoId { get; set; }
    public string? TerminoNombre { get; set; }
    public List<int> AcompanamientoIds { get; set; } = new();
    public List<string> AcompanamientoNombres { get; set; } = new();
    public bool TieneDetalle => !string.IsNullOrEmpty(TerminoNombre) || AcompanamientoNombres.Count > 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalProducto))]
    private int _cantidad;
    public ObservableCollection<ObservacionItem> Observaciones { get; } = [];
    public decimal TotalProducto => Cantidad * PrecioUnitario;
}

public partial class DataTableViewModel : ViewModelBase
{
    // 1. Casos de Uso Inyectados (Clean Architecture)
    private readonly GetAllCatalogoUseCase _getCatalogoUseCase;
    private readonly GetSessionSave _getUserUseCase;
    private readonly GetCatalogoXIdProdUseCase _getCatalogoXIdProdUseCase;
    private readonly INavigationService _navigate;
    private readonly GetAllObservacionUseCase _getAllObsUseCase;
    private readonly IDialogService _dialogService;
    private readonly IViewModelFactory _factory;
    private readonly GetAllTerminosUseCase _getTerminos;
    private readonly GetAllAcompanamientosUseCase _getAcompanamientos;
    private readonly CrearComandaUseCase _crearComanda;


    // 2. Propiedades Reactivas del Lado Izquierdo (La Cuenta)
    [ObservableProperty] private string _nombreMesero = "Cargando...";
    [ObservableProperty] private DateTime _horaApertura;
    [ObservableProperty] private decimal _totalCuenta;
    [ObservableProperty] private int _cantidadPax;
    [ObservableProperty] private Decimal _totalIpoconsumo;
    [ObservableProperty] private Decimal _totalServicio;
    [ObservableProperty] private Decimal _totalNeto;
    [ObservableProperty] private string? _numeroMesa;
    [ObservableProperty] private string? _nombreMesaCustom;
    [ObservableProperty] private int? _cantidadProd;
    [ObservableProperty] private string? _productoOn;
    [ObservableProperty] private bool _modoAnulacion;
    [ObservableProperty] private ProductoPedidoItem? _productoSeleccionado;
    [ObservableProperty] private SelectionMode _modoSeleccion = SelectionMode.Single;
    [ObservableProperty] private bool _esSeleccionMultiple;
    [ObservableProperty] private bool _mostrandoObservaciones;
    [ObservableProperty] private string? _namePanel;
    [ObservableProperty] private string? _instancia;
    [ObservableProperty] private bool _enviandoOrden;
    private int _ventaId;

    private List<TerminoModel>? _cacheTerminos;
    private List<AcompanamientoModel>? _cacheAcompanamientos;

    // Lista dinámica para los productos que el mesero va agregando a la comanda
    public ObservableCollection<ProductoPedidoItem> ProductosPedidos { get; } = new();
    public ObservableCollection<ProductoItem> ProductosCatalogo { get; } = new();
    public ObservableCollection<CategoriaItem> Categorias { get; } = new();
    public ObservableCollection<ProductoPedidoItem> ProductosSeleccionados { get; } = new();
    public ObservableCollection<ObservacionItem> Observations { get; } = new();
    private readonly Stack<object> _historialAgregados = new();

    // Constructor que resuelve las dependencias automáticamente
    public DataTableViewModel(
        GetAllCatalogoUseCase getCatalogoUseCase,
        GetSessionSave getUserUseCase,
        INavigationService navigation,
        GetAllObservacionUseCase getAllObsUseCase,
        GetCatalogoXIdProdUseCase getCatalogoXIdProdUseCase,
        IDialogService dialogService,
        IViewModelFactory factory,
        GetAllTerminosUseCase getTerminos,
        GetAllAcompanamientosUseCase getAcompanamientos,
        CrearComandaUseCase crearComanda
        )
    {
        _getCatalogoUseCase = getCatalogoUseCase;
        _getUserUseCase = getUserUseCase;
        _getAllObsUseCase = getAllObsUseCase;
        _getCatalogoXIdProdUseCase = getCatalogoXIdProdUseCase;
        _navigate = navigation;
        _dialogService = dialogService;
        _factory = factory;
        _getTerminos = getTerminos;
        _getAcompanamientos = getAcompanamientos;
        _crearComanda = crearComanda;

        // Inicializar datos básicos de la cuenta
        HoraApertura = DateTime.Now;
        _ = CargarDatosIniciales();
    }

    public void Initialize(VentaModel venta, int cantidad)
    {
        _ventaId = venta.VentaId;
        NumeroMesa = venta.NumeroMesa.ToString();
        CantidadPax = cantidad;
        Instancia = venta.Instancia.ToString();
        NombreMesaCustom = venta.Alias;
        NombreMesero = venta.Mesero;
    }

    private async Task CargarDatosIniciales()
    {

        var catalogo = await _getCatalogoUseCase.Execute();

        foreach (var item in catalogo)
        {
            Categorias.Add(new CategoriaItem { Id = item.Id, Nombre = item.Name });
        }

        _ = ChangeCategoria(Categorias.FirstOrDefault()?.Id ?? 0);
    }


    [RelayCommand]
    private async Task ChangeCategoria(int id)
    {
        ProductosCatalogo.Clear();
        MostrandoObservaciones = false;
        NamePanel = "PRODUCTOS DISPONIBLES";

        var ListProducts = await _getCatalogoXIdProdUseCase.Execute(id);
        foreach (var item in ListProducts)
        {
            ProductosCatalogo.Add(new ProductoItem
            {
                Id = item.Id.ToString(),
                Nombre = item.Name,
                Precio = (decimal)item.Price,
                TieneTerminos = item.TieneTerminos,
                TieneAcompanamientos = item.TieneAcompanamientos,
                MaxAccompaniments = item.MaxAccompaniments
            });
        }
    }

    [RelayCommand]
    private void ChangeProd(int cantidad)
    {
        CantidadProd = cantidad;
    }

    public string TextoBotonAnular =>
    ModoSeleccion == SelectionMode.Multiple ? "CONFIRMAR ANULACIÓN" : "ANULAR";

    private void ActualizarTotalCuenta()
    {
        decimal baseProductos = ProductosPedidos.Sum(item =>
        item.TotalProducto +
        item.Observaciones.Sum(obs => obs.Precio ?? 0));

        TotalIpoconsumo = baseProductos - (baseProductos / 1.08m);

        TotalServicio = baseProductos * 0.10m;
        TotalNeto = baseProductos;

        TotalCuenta = baseProductos + TotalServicio;
    }


    [RelayCommand]
    private async Task AgregarProducto(ProductoItem productoSeleccionado)
    {
        if (productoSeleccionado == null) return;

        if (CantidadProd == null || CantidadProd <= 0) CantidadProd = 1;

        // 1. Término (solo si el plato lo admite)
        TerminoModel? termino = null;
        if (productoSeleccionado.TieneTerminos)
        {
            _cacheTerminos ??= (await _getTerminos.Execute()).ToList();
            var vmTermino = new ElegirTerminoViewModel(productoSeleccionado.Nombre, _cacheTerminos);
            termino = await _dialogService.ShowDialogAsync<ElegirTerminoViewModel, TerminoModel?>(vmTermino);
            if (termino == null) return; // cancelado: no se agrega
        }

        // 2. Acompañamientos (solo si el plato los admite)
        List<AcompanamientoModel> lados = new();
        if (productoSeleccionado.TieneAcompanamientos)
        {
            _cacheAcompanamientos ??= (await _getAcompanamientos.Execute()).Where(a => a.Active).ToList();
            var vmLados = new ElegirAcompanamientosViewModel(
                productoSeleccionado.Nombre,
                _cacheAcompanamientos,
                productoSeleccionado.MaxAccompaniments);
            var elegidos = await _dialogService.ShowDialogAsync<ElegirAcompanamientosViewModel, List<AcompanamientoModel>?>(vmLados);
            if (elegidos == null) return; // cancelado: no se agrega
            lados = elegidos;
        }

        // 3. Al carrito (lado izquierdo)
        var producto = new ProductoPedidoItem
        {
            Indice = ProductosPedidos.Count + 1,
            Id = productoSeleccionado.Id,
            Nombre = productoSeleccionado.Nombre,
            PrecioUnitario = productoSeleccionado.Precio,
            Cantidad = CantidadProd ?? 1,
            TerminoId = termino?.Id,
            TerminoNombre = termino?.Termino,
            AcompanamientoIds = lados.Select(l => l.Id).ToList(),
            AcompanamientoNombres = lados.Select(l => l.Name).ToList()
        };

        ProductosPedidos.Add(producto);
        ProductoOn = producto.Indice.ToString() ?? string.Empty;
        _historialAgregados.Push(producto);
        ActualizarTotalCuenta();
        CantidadProd = null;
    }

    [RelayCommand]
    private void Teclado(string numero)
    {
        if (CantidadProd == null) CantidadProd = int.Parse(numero);
        else CantidadProd = int.Parse(CantidadProd.ToString() + numero);
    }

    [RelayCommand]
    private void BorrarNumero()
    {
        CantidadProd = null;
    }

    [RelayCommand]
    private void AlternarModoSeleccion()
    {
        if (ModoSeleccion == SelectionMode.Single)
        {
            ModoSeleccion = SelectionMode.Multiple | SelectionMode.Toggle;
            EsSeleccionMultiple = true;
            return;
        }


        if (ProductosSeleccionados.Count > 0)
        {
            var productosParaEliminar = ProductosSeleccionados
                .Cast<ProductoPedidoItem>()
                .ToList();

            foreach (var producto in productosParaEliminar)
            {
                ProductosPedidos.Remove(producto);
            }
            ProductosSeleccionados.Clear();
            LastSelect();
        }
        else
        {
            // No seleccionó nada: elimina lo último agregado
            EliminarUltimoAgregado();
        }


        ActualizarTotalCuenta();
        ModoSeleccion = SelectionMode.Single;
        EsSeleccionMultiple = false;
    }

    private void LastSelect()
    {
        var ultimo = ProductosPedidos.LastOrDefault();

        System.Console.WriteLine(ultimo?.Indice.ToString());
        if (ultimo == null) return;
        ProductoOn = ultimo?.Indice.ToString();
    }
    private void EliminarUltimoAgregado()
    {
        if (_historialAgregados.Count == 0)
            return;

        var ultimo = _historialAgregados.Pop();

        switch (ultimo)
        {
            case ProductoPedidoItem producto:
                ProductosPedidos.Remove(producto);
                break;

            case ObservacionItem observacion:

                var productoPadre = ProductosPedidos
                    .FirstOrDefault(p => p.Observaciones.Contains(observacion));

                productoPadre?.Observaciones.Remove(observacion);

                break;
        }

        ActualizarTotalCuenta();
    }

    [RelayCommand]
    private async Task ChargerObservationsAsync()
    {
        NamePanel = "OBSERVACIONES";
        Observations.Clear();
        MostrandoObservaciones = true;

        var observacion = await _getAllObsUseCase.Execute();
        foreach (var item in observacion)
        {
            Observations.Add(new ObservacionItem
            {
                Item = item.Observacion,
                Precio = item.Precio
            });
        }
    }

    [RelayCommand]
    private void AgregarObservacion(ObservacionItem observacion)
    {
        var producto = ProductosPedidos.FirstOrDefault(x => x.Indice.ToString() == ProductoOn);

        if (producto == null)
            return;
        observacion.Cantidad = CantidadProd ?? null;

        if (observacion.Precio != null && CantidadProd == null) observacion.Cantidad = 1;

        producto.Observaciones.Add(observacion);
        _historialAgregados.Push(observacion);
        ActualizarTotalCuenta();
        CantidadProd = null;
    }

    [RelayCommand]
    private async Task EnviarOrden()
    {
        if (EnviandoOrden || ProductosPedidos.Count == 0 || _ventaId == 0) return;

        try
        {
            EnviandoOrden = true;
            var meseroId = _getUserUseCase.Execute().UserId;

            var items = ProductosPedidos.Select(p => new ComandaItemRequest
            {
                ProductoId = int.Parse(p.Id),
                Cantidad = p.Cantidad,
                TerminoId = p.TerminoId,
                AcompanamientoIds = p.AcompanamientoIds.ToList()
            }).ToList();

            await _crearComanda.Execute(_ventaId, meseroId, items);

            ProductosPedidos.Clear();
            _historialAgregados.Clear();
            ActualizarTotalCuenta();
        }
        finally
        {
            EnviandoOrden = false;
        }
    }

    [RelayCommand]
    private void Cancelar()
    {
        _navigate.NavigateTo<LoginViewModel>();
    }

    [RelayCommand]
    private void Volver()
    {
        _navigate.NavigateTo<TablesViewModel>();
    }

}