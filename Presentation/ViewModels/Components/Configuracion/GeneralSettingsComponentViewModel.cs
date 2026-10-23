using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Application.Services;
using MiComanderaApp.Core.Application.UseCases.Print;
using MiComanderaApp.Core.Domain.Models;

namespace MiComanderaApp.ViewModels.Components.Configuracion;

public partial class GeneralSettingsComponentViewModel : ViewModelBase
{
    private readonly GeneralSettingsStore _store;
    private readonly GetImpresorasUseCase _getImpresoras;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isSaving;
    [ObservableProperty] private string? _mensaje;
    [ObservableProperty] private bool _esError;
    [ObservableProperty] private DateTime _updatedAt;

    public bool TieneMensaje => !string.IsNullOrWhiteSpace(Mensaje);
    public bool MensajeExito => TieneMensaje && !EsError;
    public bool MensajeError => TieneMensaje && EsError;

    partial void OnMensajeChanged(string? value)
    {
        OnPropertyChanged(nameof(TieneMensaje));
        OnPropertyChanged(nameof(MensajeExito));
        OnPropertyChanged(nameof(MensajeError));
    }

    partial void OnEsErrorChanged(bool value)
    {
        OnPropertyChanged(nameof(MensajeExito));
        OnPropertyChanged(nameof(MensajeError));
    }

    // ===== Datos del negocio =====
    [ObservableProperty] private string _businessName = string.Empty;
    [ObservableProperty] private string _taxId = string.Empty;
    [ObservableProperty] private string _address = string.Empty;
    [ObservableProperty] private string _phone = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string? _logoUrl;

    // ===== Moneda =====
    [ObservableProperty] private string _currencySymbol = string.Empty;
    [ObservableProperty] private string _currencyCode = string.Empty;
    [ObservableProperty] private int _decimalPlaces = 2;

    // ===== Impuesto al consumo =====
    [ObservableProperty] private bool _taxEnabled;
    [ObservableProperty] private decimal _taxPercentage;
    [ObservableProperty] private bool _taxIncludedInPrice;
    [ObservableProperty] private string _taxLabel = string.Empty;

    // ===== Servicio / propina =====
    [ObservableProperty] private bool _serviceChargeEnabled;
    [ObservableProperty] private decimal _serviceChargePercentage;
    [ObservableProperty] private string _serviceChargeLabel = string.Empty;

    // ===== Ticket =====
    [ObservableProperty] private int _ticketWidthChars;
    [ObservableProperty] private string _ticketFooter = string.Empty;

    // ===== Operación =====
    [ObservableProperty] private int _defaultPax;
    [ObservableProperty] private int _defaultPrinterPort;

    // ===== Impresora de facturas (caja) =====
    [ObservableProperty] private int? _defaultInvoicePrinterId;
    public ObservableCollection<ImpresoraModel> Impresoras { get; } = new();

    /// <summary>Opción "Preguntar siempre" para el selector de impresora de facturas.</summary>
    public bool FacturaSinImpresoraFija => DefaultInvoicePrinterId is null or <= 0;

    /// <summary>Impresora elegida en el combo. Null = preguntar siempre.</summary>
    public ImpresoraModel? ImpresoraFacturaSeleccionada
    {
        get => DefaultInvoicePrinterId is int id && id > 0
            ? Impresoras.FirstOrDefault(i => i.Id == id)
            : null;
        set
        {
            DefaultInvoicePrinterId = value?.Id;
            OnPropertyChanged(nameof(ImpresoraFacturaSeleccionada));
        }
    }

    public GeneralSettingsComponentViewModel(GeneralSettingsStore store, GetImpresorasUseCase getImpresoras)
    {
        _store = store;
        _getImpresoras = getImpresoras;
        _ = CargarAsync();
    }

    [RelayCommand]
    private async Task Cargar()
    {
        await CargarAsync();
    }

    private async Task CargarAsync()
    {
        try
        {
            IsLoading = true;
            Mensaje = null;
            EsError = false;

            var settings = await _store.RefreshAsync();
            if (settings == null) return;

            BusinessName = settings.BusinessName;
            TaxId = settings.TaxId;
            Address = settings.Address;
            Phone = settings.Phone;
            Email = settings.Email;
            LogoUrl = settings.LogoUrl;

            CurrencySymbol = settings.CurrencySymbol;
            CurrencyCode = settings.CurrencyCode;
            DecimalPlaces = settings.DecimalPlaces;

            TaxEnabled = settings.TaxEnabled;
            TaxPercentage = settings.TaxPercentage;
            TaxIncludedInPrice = settings.TaxIncludedInPrice;
            TaxLabel = settings.TaxLabel;

            ServiceChargeEnabled = settings.ServiceChargeEnabled;
            ServiceChargePercentage = settings.ServiceChargePercentage;
            ServiceChargeLabel = settings.ServiceChargeLabel;

            TicketWidthChars = settings.TicketWidthChars;
            TicketFooter = settings.TicketFooter;

            DefaultPax = settings.DefaultPax;
            DefaultPrinterPort = settings.DefaultPrinterPort;
            DefaultInvoicePrinterId = settings.DefaultInvoicePrinterId;

            UpdatedAt = settings.UpdatedAt;

            await CargarImpresorasAsync();
        }
        catch (Exception ex)
        {
            EsError = true;
            Mensaje = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task Guardar()
    {
        try
        {
            IsSaving = true;
            Mensaje = null;
            EsError = false;

            var request = new GeneralSettingsRequest
            {
                BusinessName = BusinessName,
                TaxId = TaxId,
                Address = Address,
                Phone = Phone,
                Email = Email,
                LogoUrl = string.IsNullOrWhiteSpace(LogoUrl) ? null : LogoUrl,

                CurrencySymbol = CurrencySymbol,
                CurrencyCode = CurrencyCode,
                DecimalPlaces = DecimalPlaces,

                TaxEnabled = TaxEnabled,
                TaxPercentage = TaxPercentage,
                TaxIncludedInPrice = TaxIncludedInPrice,
                TaxLabel = TaxLabel,

                ServiceChargeEnabled = ServiceChargeEnabled,
                ServiceChargePercentage = ServiceChargePercentage,
                ServiceChargeLabel = ServiceChargeLabel,

                TicketWidthChars = TicketWidthChars,
                TicketFooter = TicketFooter,

                DefaultPax = DefaultPax,
                DefaultPrinterPort = DefaultPrinterPort,
                DefaultInvoicePrinterId = DefaultInvoicePrinterId > 0 ? DefaultInvoicePrinterId : null,
            };

            var updated = await _store.SaveAsync(request);
            if (updated != null)
                UpdatedAt = updated.UpdatedAt;

            Mensaje = "Configuración guardada correctamente.";
        }
        catch (Exception ex)
        {
            EsError = true;
            Mensaje = ex.Message;
        }
        finally
        {
            IsSaving = false;
        }
    }

    partial void OnDefaultInvoicePrinterIdChanged(int? value)
    {
        OnPropertyChanged(nameof(FacturaSinImpresoraFija));
        OnPropertyChanged(nameof(ImpresoraFacturaSeleccionada));
    }

    /// <summary>Vuelve a "Preguntar siempre" (sin impresora fija de facturas).</summary>
    [RelayCommand]
    private void QuitarImpresoraFactura()
    {
        DefaultInvoicePrinterId = null;
    }

    /// <summary>Carga las impresoras activas para elegir la de facturas.</summary>
    private async Task CargarImpresorasAsync()
    {
        try
        {
            var impresoras = await _getImpresoras.Execute();
            Impresoras.Clear();
            foreach (var impresora in impresoras.Where(i => i.Active))
            {
                Impresoras.Add(impresora);
            }

            // Si aún no hay una impresora de facturas elegida, se preselecciona
            // la que se llame "caja" o, si solo hay una, esa misma.
            if (FacturaSinImpresoraFija)
            {
                var sugerida = Impresoras.FirstOrDefault(i =>
                        i.Name.Contains("caja", StringComparison.OrdinalIgnoreCase))
                    ?? (Impresoras.Count == 1 ? Impresoras[0] : null);

                if (sugerida != null)
                    DefaultInvoicePrinterId = sugerida.Id;
            }

            // Refrescar la selección una vez cargada la lista.
            OnPropertyChanged(nameof(ImpresoraFacturaSeleccionada));
        }
        catch
        {
            // Si falla la carga de impresoras se mantiene la lista vacía.
        }
    }
}
