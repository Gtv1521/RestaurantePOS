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
using MiComanderaApp.Core.Application.UseCases.Catalogo;
using MiComanderaApp.Core.Application.UseCases.Session;
using MiComanderaApp.Interfaces;

namespace MiComanderaApp.ViewModels.Mesas;


public class ProductoItem
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
}

public class CategoriaItem
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}
public partial class ObservacionItem : ObservableObject
{
    // public int Id { get; set; }
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
        GetCatalogoXIdProdUseCase getCatalogoXIdProdUseCase
        )
    {
        _getCatalogoUseCase = getCatalogoUseCase;
        _getUserUseCase = getUserUseCase;
        _getCatalogoXIdProdUseCase = getCatalogoXIdProdUseCase;
        _navigate = navigation;



        // Inicializar datos básicos de la cuenta
        HoraApertura = DateTime.Now;
        _ = CargarDatosIniciales();
    }

    public void Initialize(int Table, int Pax)
    {
        NumeroMesa = Table.ToString();
        CantidadPax = Pax;
    }

    private async Task CargarDatosIniciales()
    {
        // Recuperamos el mesero logueado usando el caso de uso de la sesión global
        var usuarioLogueado = _getUserUseCase.Execute();
        if (usuarioLogueado != null)
        {
            NombreMesero = usuarioLogueado.NombreCompleto;
        }

        var catalogo = await _getCatalogoUseCase.Execute();

        foreach (var item in catalogo)
        {
            Categorias.Add(new CategoriaItem { Id = item.Id.ToString(), Nombre = item.Name });
        }

        _ = ChangeCategoria(Categorias.FirstOrDefault()?.Id!);
    }


    [RelayCommand]
    private async Task ChangeCategoria(string id)
    {
        ProductosCatalogo.Clear();
        MostrandoObservaciones = false;
        NamePanel = "PRODUCTOS DISPONIBLES";

        var ListProducts = await _getCatalogoXIdProdUseCase.Execute(id);
        foreach (var item in ListProducts)
        {
            ProductosCatalogo.Add(new ProductoItem { Id = item.Id.ToString(), Nombre = item.Name, Precio = (decimal)item.Price });
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

        // El precio ya incluye el 8% de impuesto al consumo
        TotalIpoconsumo = baseProductos - (baseProductos / 1.08m);

        // El servicio sí se calcula aparte
        TotalServicio = baseProductos * 0.10m;
        TotalNeto = baseProductos;

        // Solo se suma el servicio, porque el impuesto ya está incluido
        TotalCuenta = baseProductos + TotalServicio;
    }


    [RelayCommand]
    private void AgregarProducto(ProductoItem productoSeleccionado)
    {
        if (productoSeleccionado == null) return;

        if (CantidadProd == null || CantidadProd <= 0) CantidadProd = 1;
        var producto = new ProductoPedidoItem
        {
            Indice = ProductosPedidos.Count + 1,
            Id = productoSeleccionado.Id,
            Nombre = productoSeleccionado.Nombre,
            PrecioUnitario = productoSeleccionado.Precio,
            Cantidad = CantidadProd ?? 1
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
    private void ChargerObservations()
    {
        NamePanel = "OBSERVACIONES";
        Observations.Clear();
        MostrandoObservaciones = true;

        var observacion = new List<ObservacionItem>()
        {
            new ObservacionItem
            {
                Item = "Sin",
                Precio = null
            },
            new ObservacionItem
            {
                Item = "Con",
                Precio = null
            },
            new ObservacionItem
            {
                Item = "Sal",
                Precio = null
            },
            new ObservacionItem
            {
                Item = "Lechuga",
                Precio = null
            },
            new ObservacionItem
            {
                Item = "Tomate",
                Precio = null
            },
            new ObservacionItem
            {
                Item = "Vinagre",
                Precio = null
            },
            new ObservacionItem
            {
                Item = "Salsa",
                Precio = null
            },
            new ObservacionItem
            {
                Item = "Cebolla",
                Precio = null
            },
            new ObservacionItem
            {
                Item = "Ya",
                Precio = null
            },
            new ObservacionItem
            {
                Item = "Con Fuertes",
                Precio = null
            },
            new ObservacionItem
            {
                Item = "Gratinado",
                Precio = 2000
            },
            new ObservacionItem
            {
                Item = "Con guacamole",
                Precio = 3000
            },

        };


        foreach (var item in observacion)
        {
            Observations.Add(item);
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
    private void EnviarOrden()
    {
        System.Console.WriteLine("Orden enviada");
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