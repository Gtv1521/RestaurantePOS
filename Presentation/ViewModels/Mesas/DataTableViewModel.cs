using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using MiComanderaApp.Core.Application.UseCases.Catalogo;
using MiComanderaApp.Core.Application.UseCases.Session;

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

public partial class ProductoPedidoItem : ObservableObject
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalProducto))]
    private int _cantidad;
    public decimal TotalProducto => Cantidad * PrecioUnitario;
}

public partial class DataTableViewModel : ViewModelBase
{
    // 1. Casos de Uso Inyectados (Clean Architecture)
    private readonly GetAllCatalogoUseCase _getCatalogoUseCase;
    private readonly GetSessionSave _getUserUseCase;
    private readonly GetCatalogoXIdProdUseCase _getCatalogoXIdProdUseCase;


    // 2. Propiedades Reactivas del Lado Izquierdo (La Cuenta)
    [ObservableProperty] private string _nombreMesero = "Cargando...";
    [ObservableProperty] private DateTime _horaApertura;
    [ObservableProperty] private decimal _totalCuenta;
    [ObservableProperty] private int _cantidadPax;
    [ObservableProperty] private Decimal _totalIpoconsumo;
    [ObservableProperty] private Decimal _totalServicio;
    [ObservableProperty] private string? _numeroMesa;
    [ObservableProperty] private string? _nombreMesaCustom;
    [ObservableProperty] private int? _cantidadProd;
    [ObservableProperty] private bool _modoAnulacion;
    [ObservableProperty] private ProductoPedidoItem? _productoSeleccionado;



    // Lista dinámica para los productos que el mesero va agregando a la comanda
    public ObservableCollection<ProductoPedidoItem> ProductosPedidos { get; } = new();
    public ObservableCollection<ProductoItem> ProductosCatalogo { get; } = new();
    public ObservableCollection<CategoriaItem> Categorias { get; } = new();
    public ObservableCollection<ProductoPedidoItem> ProductosSeleccionados { get; } = new();

    // Constructor que resuelve las dependencias automáticamente
    public DataTableViewModel(
        GetAllCatalogoUseCase getCatalogoUseCase,
        GetSessionSave getUserUseCase,
        GetCatalogoXIdProdUseCase getCatalogoXIdProdUseCase
        )
    {
        _getCatalogoUseCase = getCatalogoUseCase;
        _getUserUseCase = getUserUseCase;
        _getCatalogoXIdProdUseCase = getCatalogoXIdProdUseCase;


        // Inicializar datos básicos de la cuenta
        HoraApertura = DateTime.Now;
        _ = CargarDatosIniciales();
    }

    public void Initialize(int Table, int Pax)
    {
        NumeroMesa = Table.ToString();
        CantidadPax = Pax;

        System.Console.WriteLine($"Mesa: {NumeroMesa}, Cantidad de Personas: {CantidadPax}");

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
    ModoAnulacion ? "CONFIRMAR ANULACIÓN" : "ANULAR";

    private void ActualizarTotalCuenta()
    {
        // Supongamos que tienes la suma de la base de los productos
        decimal baseProductos = ProductosPedidos.Sum(item => item.TotalProducto);

        // ❌ INCORRECTO: C# interpreta 0.08 como double y falla
        // TotalIpoconsumo = baseProductos * 0.08; 

        // ✅ CORRECTO: La 'm' al final fuerza a que sea decimal
        TotalIpoconsumo = baseProductos * 0.08m; // Impuesto al consumo (8%)
        TotalServicio = baseProductos * 0.10m;   // Servicio/Propina (10%)

        // El cálculo final sumará todo de forma exacta sin perder centavos
        TotalCuenta = baseProductos + TotalIpoconsumo + TotalServicio;
    }

    // 4. COMANDOS (Acciones disparadas desde los botones del XAML)
    [RelayCommand]
    private void AgregarProducto(ProductoItem productoSeleccionado)
    {
        if (productoSeleccionado == null) return;

        if (CantidadProd == null) CantidadProd = 1;
        var producto = new ProductoPedidoItem
        {
            Id = productoSeleccionado.Id,
            Nombre = productoSeleccionado.Nombre,
            PrecioUnitario = productoSeleccionado.Precio,
            Cantidad = CantidadProd ?? 1
        };

        ProductosPedidos.Add(producto);

        ActualizarTotalCuenta();
        CantidadProd = null;
        ProductoSeleccionado = ProductosPedidos.Last();
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
        CantidadProd = 1;
    }

    [RelayCommand]
    private void Anular()
    {
        if (!ModoAnulacion)
        {
            // Primer click: activar selección
            ModoAnulacion = true;
            OnPropertyChanged(nameof(TextoBotonAnular));
            return;
        }


        // Segundo click: borrar seleccionados

        foreach (var item in ProductosSeleccionados.ToList())
        {
            ProductosPedidos.Remove(item);
        }

        ProductosSeleccionados.Clear();

        ModoAnulacion = false;
        OnPropertyChanged(nameof(TextoBotonAnular));
    }

}