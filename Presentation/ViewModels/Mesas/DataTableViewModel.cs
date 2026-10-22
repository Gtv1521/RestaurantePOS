using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Application.Services;
using MiComanderaApp.Core.Application.UseCases.Acompanamiento;
using MiComanderaApp.Core.Application.UseCases.Catalogo;
using MiComanderaApp.Core.Application.UseCases.Observacion;
using MiComanderaApp.Core.Application.UseCases.Print;
using MiComanderaApp.Core.Application.UseCases.Session;
using MiComanderaApp.Core.Application.UseCases.Termino;
using MiComanderaApp.Core.Application.UseCases.Venta;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.ViewModels.Dialogs.Modals;
using MiComanderaApp.ViewModels.Orders;
using MiComanderaApp.Views.Dialogs.Modals;

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

    /// <summary>
    /// True cuando el producto ya fue enviado al servidor (viene de una comanda guardada).
    /// Los productos nuevos (aún no enviados) van en false y son los únicos que se envían.
    /// </summary>
    public bool YaEnviado { get; set; }

    /// <summary>
    /// Id del ítem de comanda en la base de datos (solo si YaEnviado es true).
    /// Se usa para eliminar el producto guardado.
    /// </summary>
    public int ComandaItemId { get; set; }

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
    private readonly GetVentaDetalleUseCase _getVentaDetalle;
    private readonly ActualizarClienteUseCase _actualizarCliente;
    private readonly CerrarVentaUseCase _cerrarVenta;
    private readonly GetImpresorasUseCase _getImpresoras;
    private readonly ImprimirDocumentoUseCase _imprimirDocumento;
    private readonly ISession<SessionModel> _session;


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
    [ObservableProperty] private bool _esCaja;
    [ObservableProperty] private bool _ventaCerrada;
    private int _ventaId;
    private decimal _totalServidor;
    private bool _datosCargados;

    // ============================================================
    // ESTADO DE COBRO EN LÍNEA (sin ventana modal)
    // ============================================================
    [ObservableProperty] private bool _modoPago;
    [ObservableProperty] private decimal _pagoTotal;
    [ObservableProperty] private string _propinaTexto = "0";
    [ObservableProperty] private bool _editandoPropina;
    [ObservableProperty] private string _pagoMontoTexto = "0";
    [ObservableProperty] private string _pagoClienteNombre = string.Empty;
    [ObservableProperty] private string _pagoClienteDocumento = string.Empty;
    [ObservableProperty] private string _pagoClienteDireccion = string.Empty;
    [ObservableProperty] private string _pagoClienteTelefono = string.Empty;
    [ObservableProperty] private MetodoPagoOption? _pagoMetodoSeleccionado;
    [ObservableProperty] private string? _pagoError;
    [ObservableProperty] private bool _pagoProcesando;

    // ============================================================
    // BÚSQUEDA / CARGA DE CLIENTE (MAQUETA - sin backend todavía)
    // ============================================================
    [ObservableProperty] private string _clienteBusqueda = string.Empty;
    [ObservableProperty] private string? _clienteMensaje;
    [ObservableProperty] private bool _clienteNoEncontrado;
    [ObservableProperty] private bool _agregandoCliente;

    public bool ClienteMensajeVisible => !string.IsNullOrWhiteSpace(ClienteMensaje);
    partial void OnClienteMensajeChanged(string? value) => OnPropertyChanged(nameof(ClienteMensajeVisible));

    // MOCK temporal para la maqueta: clientes de ejemplo.
    // TODO: reemplazar por la búsqueda real cuando exista el endpoint de clientes.
    private static readonly Dictionary<string, (string Nombre, string Documento, string Direccion, string Telefono)> ClientesMock =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["12345678"] = ("Juan Pérez", "12345678", "Av. Los Álamos 123", "999111222"),
            ["87654321"] = ("María Gómez", "87654321", "Calle Las Flores 456", "988333444"),
            ["20123456789"] = ("Restaurante Demo S.A.C.", "20123456789", "Av. Industrial 500", "014567890"),
        };

    private List<TerminoModel>? _cacheTerminos;
    private List<AcompanamientoModel>? _cacheAcompanamientos;

    // Lista dinámica para los productos que el mesero va agregando a la comanda
    public ObservableCollection<ProductoPedidoItem> ProductosPedidos { get; } = new();
    public ObservableCollection<ProductoItem> ProductosCatalogo { get; } = new();
    public ObservableCollection<CategoriaItem> Categorias { get; } = new();
    public ObservableCollection<ProductoPedidoItem> ProductosSeleccionados { get; } = new();
    public ObservableCollection<ObservacionItem> Observations { get; } = new();
    private readonly Stack<object> _historialAgregados = new();

    // Métodos de pago y pagos agregados para el cobro en línea
    public ObservableCollection<MetodoPagoOption> MetodosPago { get; } = new()
    {
        new() { Nombre = "Efectivo", Etiqueta = "Efectivo", Codigo = 0, MarcaTarjeta = null, Letra = "💵" },
        new() { Nombre = "Visa", Etiqueta = "Visa", Codigo = 1, MarcaTarjeta = "Visa", Letra = "V" },
        new() { Nombre = "Mastercard", Etiqueta = "Mastercard", Codigo = 1, MarcaTarjeta = "Mastercard", Letra = "M" },
        new() { Nombre = "American Express", Etiqueta = "Amex", Codigo = 1, MarcaTarjeta = "American Express", Letra = "AM" },
        new() { Nombre = "Diners Club", Etiqueta = "Diners", Codigo = 1, MarcaTarjeta = "Diners Club", Letra = "D" },
        new() { Nombre = "Otra tarjeta", Etiqueta = "Otra", Codigo = 2, MarcaTarjeta = "Otra tarjeta", Letra = "T" },
        new() { Nombre = "Transferencia", Etiqueta = "Transf.", Codigo = 3, MarcaTarjeta = null, Letra = "↗" },
        new() { Nombre = "Yape", Etiqueta = "Yape", Codigo = 3, MarcaTarjeta = null, Letra = "Y" },
        new() { Nombre = "Plin", Etiqueta = "Plin", Codigo = 3, MarcaTarjeta = null, Letra = "P" },
    };

    public ObservableCollection<PagoItemModel> PagosAgregados { get; } = new();

    // Propiedades calculadas del cobro
    public decimal PagoMonto => decimal.TryParse((PagoMontoTexto ?? "0").Replace(',', '.'),
        NumberStyles.Any, CultureInfo.InvariantCulture, out var valor) ? valor : 0;

    // La propina se precarga con el 10% de servicio y queda editable en el panel de pagos
    public decimal PagoPropina => decimal.TryParse((PropinaTexto ?? "0").Replace(',', '.'),
        NumberStyles.Any, CultureInfo.InvariantCulture, out var propina) ? propina : 0;
    public decimal PagoValorTeclado => EditandoPropina ? PagoPropina : PagoMonto;
    public string PagoTecladoTitulo => EditandoPropina ? "PROPINA" : "VALOR A PAGAR";
    public string PagoTecladoAyuda => EditandoPropina
        ? "Digitando PROPINA · usa el teclado"
        : "Digita el valor y elige el método de pago";

    public decimal PagoTotalPagado => PagosAgregados.Sum(p => p.Monto);
    public bool PagoPuedeConfirmar => PagosAgregados.Count > 0 && PagoTotalPagado >= PagoTotal + PagoPropina && !PagoProcesando;
    public string PagoTotalTexto => $"${PagoTotal + PagoPropina:N2}";
    public string PagoPagadoTexto => $"${PagoTotalPagado:N2}";
    public string PagoPendienteTexto => $"${Math.Max(0, PagoTotal + PagoPropina - PagoTotalPagado):N2}";

    // Restante o cambio (si pagan de más)
    public decimal PagoRestanteValor => PagoTotal + PagoPropina - PagoTotalPagado;
    public bool PagoEsCambio => PagoRestanteValor < 0;
    public string PagoRestanteTexto => $"${Math.Abs(PagoRestanteValor):N2}";

    partial void OnPagoMontoTextoChanged(string value)
    {
        OnPropertyChanged(nameof(PagoMonto));
        OnPropertyChanged(nameof(PagoValorTeclado));
    }
    partial void OnPropinaTextoChanged(string value)
    {
        OnPropertyChanged(nameof(PagoPropina));
        OnPropertyChanged(nameof(PagoValorTeclado));

        if (ModoPago)
        {
            // Reflejar la propina en la cuenta: SERVICIO (PROPINA) y TOTAL A PAGAR
            ActualizarTotalCuenta();

            // No mantener el monto anterior; el valor entrante siempre debe quedar en cero
            // Se deja a cero tras cualquier cambio de propina.
        }

        NotificarResumenPago();
    }
    partial void OnEditandoPropinaChanged(bool value)
    {
        OnPropertyChanged(nameof(PagoValorTeclado));
        OnPropertyChanged(nameof(PagoTecladoTitulo));
        OnPropertyChanged(nameof(PagoTecladoAyuda));
    }
    partial void OnPagoTotalChanged(decimal value) => NotificarResumenPago();
    partial void OnPagoProcesandoChanged(bool value) => OnPropertyChanged(nameof(PagoPuedeConfirmar));

    private void NotificarResumenPago()
    {
        OnPropertyChanged(nameof(PagoTotalPagado));
        OnPropertyChanged(nameof(PagoPagadoTexto));
        OnPropertyChanged(nameof(PagoPendienteTexto));
        OnPropertyChanged(nameof(PagoTotalTexto));
        OnPropertyChanged(nameof(PagoPuedeConfirmar));
        OnPropertyChanged(nameof(PagoRestanteValor));
        OnPropertyChanged(nameof(PagoEsCambio));
        OnPropertyChanged(nameof(PagoRestanteTexto));
    }

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
        CrearComandaUseCase crearComanda,
        GetVentaDetalleUseCase getVentaDetalle,
        ActualizarClienteUseCase actualizarCliente,
        CerrarVentaUseCase cerrarVenta,
        GetImpresorasUseCase getImpresoras,
        ImprimirDocumentoUseCase imprimirDocumento,
        ISession<SessionModel> session
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
        _getVentaDetalle = getVentaDetalle;
        _actualizarCliente = actualizarCliente;
        _cerrarVenta = cerrarVenta;
        _getImpresoras = getImpresoras;
        _imprimirDocumento = imprimirDocumento;
        _session = session;

        // El cajero (y el administrador) ven las acciones de caja en la mesa
        var rol = _getUserUseCase.Execute()?.Rol ?? string.Empty;
        EsCaja = rol is "Cajero" or "Administrador";

        // Inicializar datos básicos de la cuenta
        HoraApertura = DateTime.Now;
        _ = CargarDatosIniciales();
    }

    public void Initialize(VentaModel venta, int cantidad)
    {
        var mismaVenta = _ventaId == venta.VentaId;

        _ventaId = venta.VentaId;
        NumeroMesa = venta.NumeroMesa.ToString();
        CantidadPax = cantidad;
        Instancia = venta.Instancia.ToString();
        NombreMesaCustom = venta.Alias;
        NombreMesero = venta.Mesero;

        // Si es una venta distinta (o aún no cargamos nada), traemos lo ya enviado al servidor.
        // Si es la misma venta que ya estaba abierta, conservamos lo que el usuario tenga en curso.
        if (!mismaVenta)
        {
            _datosCargados = false;
            _totalServidor = 0;
            ProductosPedidos.Clear();
            _historialAgregados.Clear();
            _ = CargarVentaAsync();
        }
    }

    /// <summary>
    /// Trae del servidor todo lo enviado previamente para la venta (productos,
    /// observaciones, términos y acompañamientos) y lo refleja en la lista de pedidos.
    /// </summary>
    public async Task CargarVentaAsync()
    {
        if (_ventaId == 0) return;

        try
        {
            var detalle = await _getVentaDetalle.Execute(_ventaId);

            _datosCargados = true;
            _totalServidor = detalle.TotalAmount;
            NombreMesero = detalle.NombreMesero;

            ProductosPedidos.Clear();
            _historialAgregados.Clear();

            foreach (var comanda in detalle.Comandas)
            {
                foreach (var item in comanda.Items)
                {
                    if (item.Cantidad <= 0) continue;

                    var pedido = new ProductoPedidoItem
                    {
                        Indice = ProductosPedidos.Count + 1,
                        Id = item.ProductoId.ToString(),
                        Nombre = item.NombreProducto,
                        PrecioUnitario = item.PrecioUnitario,
                        Cantidad = item.Cantidad,
                        TerminoNombre = item.Termino,
                        AcompanamientoIds = item.AcompanamientoIds?.ToList() ?? new(),
                        AcompanamientoNombres = await ObtenerNombresAcompanamientos(item.AcompanamientoIds),
                        YaEnviado = true,
                        ComandaItemId = item.Id
                    };

                    foreach (var obsId in item.Observaciones ?? new())
                    {
                        var observacion = await ObtenerObservacion(obsId);
                        if (observacion != null)
                            pedido.Observaciones.Add(observacion);
                    }

                    ProductosPedidos.Add(pedido);
                }
            }

            ActualizarTotalCuenta();
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"No se pudo cargar la venta {_ventaId}: {ex.Message}");
        }
    }

    private async Task<List<string>> ObtenerNombresAcompanamientos(List<int>? ids)
    {
        var nombres = new List<string>();
        if (ids == null || ids.Count == 0) return nombres;

        foreach (var id in ids.Distinct())
        {
            var acompanamiento = _cacheAcompanamientos?.FirstOrDefault(a => a.Id == id);
            if (acompanamiento == null)
            {
                _cacheAcompanamientos ??= (await _getAcompanamientos.Execute()).Where(a => a.Active).ToList();
                acompanamiento = _cacheAcompanamientos.FirstOrDefault(a => a.Id == id);
            }
            if (acompanamiento != null) nombres.Add(acompanamiento.Name);
        }

        return nombres;
    }

    private async Task<ObservacionItem?> ObtenerObservacion(int id)
    {
        if (id <= 0) return null;

        var existente = Observations.FirstOrDefault(o => o.Id == id);
        if (existente != null) return existente;

        var todas = await _getAllObsUseCase.Execute();
        var observacion = todas.FirstOrDefault(o => o.Id == id);
        if (observacion == null) return null;

        var item = new ObservacionItem
        {
            Id = observacion.Id,
            Item = observacion.Observacion,
            Precio = observacion.Precio
        };
        Observations.Add(item);
        return item;
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

    private decimal CalcularBaseCuenta()
    {
        decimal baseProductos = ProductosPedidos.Sum(item =>
            item.TotalProducto +
            item.Observaciones.Sum(obs => obs.Precio ?? 0));

        // Si no hay productos locales (p. ej. el cajero reabre una mesa),
        // usamos el total que devuelve el servidor.
        return baseProductos > 0 ? baseProductos : _totalServidor;
    }

    private void ActualizarTotalCuenta()
    {
        decimal baseProductos = CalcularBaseCuenta();

        TotalIpoconsumo = baseProductos - (baseProductos / 1.08m);
        TotalNeto = baseProductos - TotalIpoconsumo;

        // El servicio es la propina: durante el cobro sigue a la propina editable
        // (botones "Modificar propina" / "Quitar propina"); fuera del cobro es el 10%.
        TotalServicio = ModoPago ? PagoPropina : TotalNeto * 0.10m;
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
        // En modo cobro el teclado escribe el valor a pagar (o la propina)
        if (ModoPago)
        {
            if (EditandoPropina)
            {
                PropinaTexto = (string.IsNullOrEmpty(PropinaTexto) || PropinaTexto == "0")
                    ? numero
                    : PropinaTexto + numero;
                return;
            }

            PagoMontoTexto = (string.IsNullOrEmpty(PagoMontoTexto) || PagoMontoTexto == "0")
                ? numero
                : PagoMontoTexto + numero;
            return;
        }

        if (CantidadProd == null) CantidadProd = int.Parse(numero);
        else CantidadProd = int.Parse(CantidadProd.ToString() + numero);
    }

    [RelayCommand]
    private void PuntoDecimal()
    {
        if (!ModoPago) return;

        if (EditandoPropina)
        {
            if (string.IsNullOrEmpty(PropinaTexto) || PropinaTexto == "0")
                PropinaTexto = "0.";
            else if (!PropinaTexto.Contains('.'))
                PropinaTexto += ".";
            return;
        }

        if (string.IsNullOrEmpty(PagoMontoTexto) || PagoMontoTexto == "0")
        {
            PagoMontoTexto = "0.";
            return;
        }

        if (!PagoMontoTexto.Contains('.'))
            PagoMontoTexto += ".";
    }

    [RelayCommand]
    private void BorrarNumero()
    {
        // En modo cobro el teclado borra el valor a pagar (o la propina)
        if (ModoPago)
        {
            if (EditandoPropina)
            {
                if (!string.IsNullOrEmpty(PropinaTexto))
                    PropinaTexto = PropinaTexto[..^1];
                return;
            }

            if (!string.IsNullOrEmpty(PagoMontoTexto))
                PagoMontoTexto = PagoMontoTexto[..^1];
            return;
        }

        CantidadProd = null;
    }

    [RelayCommand]
    private async Task AlternarModoSeleccion()
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

            // Si alguno ya está guardado en la base de datos, se necesita autorización
            // (el Administrador no requiere PIN; cualquier otro rol sí).
            if (!await AutorizarAnulacionAsync(productosParaEliminar, 0))
            {
                ModoSeleccion = SelectionMode.Single;
                EsSeleccionMultiple = false;
                return;
            }

            await EliminarProductosAsync(productosParaEliminar);

            ProductosSeleccionados.Clear();
            LastSelect();
        }
        else
        {
            // No seleccionó nada: elimina lo último agregado
            await EliminarUltimoAgregadoAsync();
        }


        ActualizarTotalCuenta();
        ModoSeleccion = SelectionMode.Single;
        EsSeleccionMultiple = false;
    }

    /// <summary>
    /// Elimina los productos indicados de la lista. Si el producto ya estaba
    /// guardado en la base de datos, también lo elimina del servidor.
    /// </summary>
    private async Task EliminarProductosAsync(List<ProductoPedidoItem> productos)
    {
        foreach (var producto in productos)
        {
            if (producto.YaEnviado && producto.ComandaItemId > 0)
            {
                try
                {
                    await _crearComanda.EliminarItem(producto.ComandaItemId);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"No se pudo eliminar el ítem {producto.ComandaItemId} de la base de datos: {ex.Message}");
                }
            }

            ProductosPedidos.Remove(producto);
        }
    }

    /// <summary>
    /// Pide autorización cuando se intenta quitar de la lista un producto (u observación)
    /// que ya está guardado en la base de datos. Si el usuario en sesión es Administrador
    /// no se pide PIN; en caso contrario se solicita el PIN de un Administrador.
    /// Devuelve true si hay que proceder con la anulación.
    /// </summary>
    private async Task<bool> AutorizarAnulacionAsync(List<ProductoPedidoItem> productos, int cantidadObservaciones)
    {
        var enviados = productos.Where(p => p.YaEnviado).ToList();

        if (enviados.Count == 0 && cantidadObservaciones == 0)
            return true;

        // El Administrador puede anular sin PIN
        var rolActual = _getUserUseCase.Execute()?.Rol ?? string.Empty;
        if (rolActual == "Administrador")
            return true;

        string mensaje;

        if (enviados.Count > 0)
        {
            var nombres = string.Join(", ", enviados.Select(p => p.Nombre));
            mensaje = enviados.Count == 1
                ? $"El producto \"{nombres}\" ya está guardado en la base de datos.\n\n" +
                  "Para quitarlo necesitas autorización de un Administrador."
                : $"Estos productos ya están guardados en la base de datos:\n{nombres}\n\n" +
                  "Para quitarlos necesitas autorización de un Administrador.";
        }
        else
        {
            mensaje = "Esta observación ya está guardada en la base de datos.\n\n" +
                      "Para quitarla necesitas autorización de un Administrador.";
        }

        var vm = new AutorizacionPinDialogViewModel(_session);
        vm.Initialize(mensaje, "Quitar producto guardado", "Administrador");

        var autorizado = await _dialogService.ShowDialogAsync<AutorizacionPinDialogView, AutorizacionPinDialogViewModel, bool>(vm);
        return autorizado == true;
    }

    private void LastSelect()
    {
        var ultimo = ProductosPedidos.LastOrDefault();

        System.Console.WriteLine(ultimo?.Indice.ToString());
        if (ultimo == null) return;
        ProductoOn = ultimo?.Indice.ToString();
    }
    private async Task EliminarUltimoAgregadoAsync()
    {
        if (_historialAgregados.Count == 0)
            return;

        var ultimo = _historialAgregados.Peek();

        switch (ultimo)
        {
            case ProductoPedidoItem producto:
                if (!await AutorizarAnulacionAsync(new List<ProductoPedidoItem> { producto }, 0))
                    return;
                break;

            case ObservacionItem observacion:
                var productoPadre = ProductosPedidos
                    .FirstOrDefault(p => p.Observaciones.Contains(observacion));

                if (productoPadre != null && productoPadre.YaEnviado)
                {
                    if (!await AutorizarAnulacionAsync(new List<ProductoPedidoItem>(), 1))
                        return;
                }
                break;
        }

        ultimo = _historialAgregados.Pop();

        switch (ultimo)
        {
            case ProductoPedidoItem producto:
                await EliminarProductosAsync(new List<ProductoPedidoItem> { producto });
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
                Id = item.Id,
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
        if (EnviandoOrden || _ventaId == 0) return;

        // Solo se envían los productos NUEVOS (los que ya estaban guardados no se repiten)
        var nuevos = ProductosPedidos.Where(p => !p.YaEnviado).ToList();
        if (nuevos.Count == 0) return;

        try
        {
            EnviandoOrden = true;
            var meseroId = _getUserUseCase.Execute().UserId;

            var items = nuevos.Select(p => new ComandaItemRequest
            {
                ProductoId = int.Parse(p.Id),
                Cantidad = p.Cantidad,
                TerminoId = p.TerminoId,
                AcompanamientoIds = p.AcompanamientoIds.ToList(),
                Observaciones = p.Observaciones
                    .Select(o => o.Id)
                    .Where(id => id > 0)
                    .Distinct()
                    .ToList()
            }).ToList();

            await _crearComanda.Execute(_ventaId, meseroId, items);

            // Terminó el ciclo de envío: cerramos la sesión y volvemos al inicio
            await _session.LogoutAsync();
            _navigate.NavigateTo<LoginViewModel>();

        }
        finally
        {
            EnviandoOrden = false;
        }
    }

    [RelayCommand]
    private async Task Cancelar()
    {
        if (!await ConfirmarSalidaAsync("cancelar"))
            return;

        _navigate.NavigateTo<LoginViewModel>();
    }

    [RelayCommand]
    private async Task Volver()
    {
        if (!await ConfirmarSalidaAsync("volver"))
            return;

        _navigate.NavigateTo<TablesViewModel>();
    }

    /// <summary>
    /// Pide permiso cuando hay productos agregados que aún no se han enviado
    /// (guardado) al servidor. Devuelve true si hay que continuar con la acción.
    /// </summary>
    private async Task<bool> ConfirmarSalidaAsync(string accion)
    {
        var pendientes = ProductosPedidos.Where(p => !p.YaEnviado).ToList();

        if (pendientes.Count == 0)
            return true;

        string mensaje;
        if (pendientes.Count == 1)
        {
            mensaje = $"El producto \"{pendientes[0].Nombre}\" no ha sido enviado a cocina.\n\n" +
                      $"¿Seguro que quieres {accion} y perderlo?";
        }
        else
        {
            var nombres = string.Join(", ", pendientes.Select(p => p.Nombre));
            mensaje = $"Hay {pendientes.Count} productos sin enviar a cocina:\n{nombres}\n\n" +
                      $"¿Seguro que quieres {accion} y perderlos?";
        }

        var vm = new ConfirmDialogViewModel();
        vm.Initialize("Productos sin guardar", mensaje, "Sí, continuar");

        var confirmado = await _dialogService.ShowDialogAsync<ConfirmDialogView, ConfirmDialogViewModel, bool>(vm);
        return confirmado == true;
    }

    // ============================================================
    // ACCIONES DE CAJA (visibles solo para Cajero / Administrador)
    // ============================================================

    [RelayCommand]
    private async Task EditarDatosCliente()
    {
        if (_ventaId == 0) return;

        var detalle = await _getVentaDetalle.Execute(_ventaId);
        var vm = new DatosClienteDialogViewModel();
        vm.Initialize(detalle);

        var cliente = await _dialogService.ShowDialogAsync<DatosClienteDialogView, DatosClienteDialogViewModel, VentaClienteRequest?>(
            vm, new PixelPoint(300, 60));

        if (cliente != null)
            await _actualizarCliente.Execute(_ventaId, cliente);
    }

    [RelayCommand]
    private async Task Prefactura() =>
        await MostrarDocumento("PREFACTURA", DocumentosVenta.BuildPrefactura);

    [RelayCommand]
    private async Task Factura() =>
        await MostrarDocumento("FACTURA", DocumentosVenta.BuildFactura);

    [RelayCommand]
    private async Task Cobrar()
    {
        if (_ventaId == 0) return;

        // El cobro se resuelve dentro de la propia vista (panel de opciones),
        // sin abrir ninguna ventana modal.
        if (!_datosCargados)
            await CargarVentaAsync();

        var detalle = await _getVentaDetalle.Execute(_ventaId);

        // Si la venta ya está cobrada, solo volvemos a mostrar el comprobante
        if (detalle.TienePagos)
        {
            await MostrarDocumento("COMPROBANTE DE PAGO", DocumentosVenta.BuildComprobantePago, detalle);
            return;
        }

        _totalServidor = detalle.TotalAmount;
        PagoTotal = detalle.TotalAmount;
        PagoClienteNombre = detalle.ClienteNombre ?? string.Empty;
        PagoClienteDocumento = detalle.ClienteDocumento ?? string.Empty;
        PagoClienteDireccion = detalle.ClienteDireccion ?? string.Empty;
        PagoClienteTelefono = detalle.ClienteTelefono ?? string.Empty;

        // Precargamos la propina calculada (10% de servicio) sobre el total de la cuenta.
        // Queda editable con los botones "Modificar propina" / "Quitar propina".
        PropinaTexto = (CalcularBaseCuenta() * 0.10m).ToString("0.##", CultureInfo.InvariantCulture);
        EditandoPropina = false;
        PagosAgregados.Clear();
        PagoError = null;
        PagoMetodoSeleccionado = null;
        ClienteBusqueda = string.Empty;
        ClienteMensaje = null;
        ClienteNoEncontrado = false;
        AgregandoCliente = false;
        MarcarMetodoSeleccionado(MetodosPago.FirstOrDefault());
        ModoPago = true;
        // Ya en modo cobro: la cuenta refleja la propina (servicio) y el total a pagar
        PagoTotal = CalcularBaseCuenta();
        ActualizarTotalCuenta();
        // El valor del teclado (parte superior del panel de pagos) inicia siempre en cero
        PagoMontoTexto = "0";
        NotificarResumenPago();
    }

    private void MarcarMetodoSeleccionado(MetodoPagoOption? metodo)
    {
        EditandoPropina = false;

        foreach (var m in MetodosPago)
            m.Seleccionado = m == metodo;

        PagoMetodoSeleccionado = metodo;
    }

    [RelayCommand]
    private async Task SeleccionarMetodoPago(MetodoPagoOption? metodo)
    {
        if (metodo == null) return;

        MarcarMetodoSeleccionado(metodo);

        // Al elegir el método con un valor digitado, se agrega el pago automáticamente.
        if (PagoMonto > 0)
            await AgregarPago();
    }

    [RelayCommand]
    private async Task AgregarPago()
    {
        PagoError = null;
        EditandoPropina = false;

        if (PagoMetodoSeleccionado == null)
        {
            PagoError = "Selecciona un método de pago.";
            return;
        }

        if (PagoMonto <= 0)
        {
            PagoError = "El monto debe ser mayor a 0.";
            return;
        }

        PagosAgregados.Add(new PagoItemModel
        {
            MetodoNombre = PagoMetodoSeleccionado.MarcaTarjeta ?? PagoMetodoSeleccionado.Nombre,
            Monto = PagoMonto
        });

        // Siempre devolver el valor entrante (teclado superior) a cero
        PagoMontoTexto = "0";
        NotificarResumenPago();

        // Cuando el total llega a cero, se cierra la venta y se manda a imprimir.
        if (PagoTotal > 0 && !PagoProcesando && PagoTotalPagado >= PagoTotal + PagoPropina)
            await ConfirmarCobro();
    }

    [RelayCommand]
    private void QuitarPago(PagoItemModel? pago)
    {
        if (pago == null) return;

        PagosAgregados.Remove(pago);
        // Siempre devolver el valor entrante (teclado superior) a cero
        PagoMontoTexto = "0";
        NotificarResumenPago();
    }

    [RelayCommand]
    private void ModificarPropina()
    {
        PagoError = null;
        EditandoPropina = true;
        PropinaTexto = "0";
    }

    [RelayCommand]
    private void QuitarPropina()
    {
        PagoError = null;
        EditandoPropina = false;
        PropinaTexto = "0";

        // Siempre devolver el valor entrante (teclado superior) a cero
        PagoMontoTexto = "0";
        NotificarResumenPago();
    }

    [RelayCommand]
    private async Task ConfirmarCobro()
    {
        PagoError = null;

        if (PagosAgregados.Count == 0)
        {
            PagoError = "Agrega al menos un pago.";
            return;
        }

        if (PagoTotalPagado < PagoTotal + PagoPropina)
        {
            PagoError = "El monto pagado es insuficiente para cerrar la venta.";
            return;
        }

        PagoProcesando = true;
        try
        {
            // Guardar los datos del cliente si el cajero los ingresó
            if (!string.IsNullOrWhiteSpace(PagoClienteNombre))
            {
                await _actualizarCliente.Execute(_ventaId, new VentaClienteRequest
                {
                    ClienteNombre = PagoClienteNombre.Trim(),
                    ClienteDocumento = PagoClienteDocumento?.Trim(),
                    ClienteDireccion = PagoClienteDireccion?.Trim(),
                    ClienteTelefono = PagoClienteTelefono?.Trim()
                });
            }

            // Reconstruye los pagos agrupados por método/marca
            var pagos = PagosAgregados
                .GroupBy(p => p.MetodoNombre)
                .Select(g =>
                {
                    var metodo = MetodosPago.FirstOrDefault(m =>
                                    (m.MarcaTarjeta ?? m.Nombre) == g.Key)
                                ?? MetodosPago.First();
                    return new PagoRequest
                    {
                        Metodo = metodo.Codigo,
                        MarcaTarjeta = metodo.MarcaTarjeta,
                        Monto = g.Sum(x => x.Monto),
                        Referencia = metodo.MarcaTarjeta == null ? null : $"**** {DateTime.Now:HHmm}"
                    };
                })
                .ToList();

            var ok = await _cerrarVenta.Execute(new CerrarVentaRequest
            {
                VentaId = _ventaId,
                Propina = PagoPropina,
                Pagos = pagos
            });

            if (!ok)
            {
                PagoError = "No se pudo cerrar la venta.";
                return;
            }

            ModoPago = false;
            var cerrada = await _getVentaDetalle.Execute(_ventaId);
            await MostrarDocumento("COMPROBANTE DE PAGO", DocumentosVenta.BuildComprobantePago, cerrada);
            await Volver();
        }
        catch (Exception ex)
        {
            PagoError = $"Error al cobrar: {ex.Message}";
        }
        finally
        {
            PagoProcesando = false;
        }
    }

    [RelayCommand]
    private void CancelarPago()
    {
        ModoPago = false;
        PagoError = null;
        EditandoPropina = false;
        PropinaTexto = "0";
        PagosAgregados.Clear();
        ClienteBusqueda = string.Empty;
        ClienteMensaje = null;
        ClienteNoEncontrado = false;
        AgregandoCliente = false;
        ActualizarTotalCuenta();
    }

    // ============================================================
    // CLIENTE (MAQUETA): cargar uno existente o agregar uno nuevo
    // ============================================================
    [RelayCommand]
    private void CargarCliente()
    {
        var q = (ClienteBusqueda ?? string.Empty).Trim();

        if (string.IsNullOrEmpty(q))
        {
            ClienteNoEncontrado = false;
            ClienteMensaje = "Ingresa un documento o nombre para buscar.";
            return;
        }

        // MOCK: búsqueda simulada sobre la lista local de ejemplo.
        var encontrado = ClientesMock.FirstOrDefault(kv =>
            kv.Key.Equals(q, StringComparison.OrdinalIgnoreCase) ||
            kv.Value.Nombre.Contains(q, StringComparison.OrdinalIgnoreCase));

        if (encontrado.Key is null)
        {
            ClienteNoEncontrado = true;
            ClienteMensaje = "Cliente no encontrado. Puedes agregarlo.";
            return;
        }

        PagoClienteNombre = encontrado.Value.Nombre;
        PagoClienteDocumento = encontrado.Value.Documento;
        PagoClienteDireccion = encontrado.Value.Direccion;
        PagoClienteTelefono = encontrado.Value.Telefono;
        ClienteNoEncontrado = false;
        ClienteMensaje = $"Cliente cargado: {encontrado.Value.Nombre}";
    }

    [RelayCommand]
    private void AgregarCliente()
    {
        // Muestra el formulario y oculta la búsqueda.
        AgregandoCliente = true;
        PagoClienteNombre = string.Empty;
        PagoClienteDocumento = string.IsNullOrWhiteSpace(ClienteBusqueda) ? string.Empty : ClienteBusqueda.Trim();
        PagoClienteDireccion = string.Empty;
        PagoClienteTelefono = string.Empty;
        ClienteNoEncontrado = false;
        ClienteMensaje = "Ingresa los datos del nuevo cliente.";
    }

    [RelayCommand]
    private void VolverBuscarCliente()
    {
        // Vuelve al modo búsqueda y oculta el formulario.
        AgregandoCliente = false;
        ClienteMensaje = null;
    }

    private async Task MostrarDocumento(string titulo, Func<VentaDetalleModel, string> constructor, VentaDetalleModel? detalle = null)
    {
        if (_ventaId == 0) return;

        detalle ??= await _getVentaDetalle.Execute(_ventaId);
        var texto = constructor(detalle);

        var vm = new PrintPreviewDialogViewModel(_getImpresoras, _imprimirDocumento);
        vm.Initialize(titulo, texto);

        await _dialogService.ShowDialogAsync<PrintPreviewDialogView, PrintPreviewDialogViewModel, bool?>(
            vm, new PixelPoint(300, 10));
    }

}