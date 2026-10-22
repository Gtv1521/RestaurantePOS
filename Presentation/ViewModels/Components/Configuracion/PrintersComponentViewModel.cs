using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Application.Services;
using MiComanderaApp.Core.Application.UseCases.Catalogo;
using MiComanderaApp.Core.Application.UseCases.Configuracion;
using MiComanderaApp.Core.Application.UseCases.Print;
using MiComanderaApp.Core.Domain.Models;

namespace MiComanderaApp.ViewModels.Components.Configuracion;

public partial class PrintersComponentViewModel : ViewModelBase
{
    private readonly GetImpresorasUseCase _get;
    private readonly CreateImpresoraUseCase _create;
    private readonly UpdateImpresoraUseCase _update;
    private readonly DeleteImpresoraUseCase _delete;
    private readonly DiscoverImpresorasUseCase _discover;
    private readonly RegisterDiscoveredImpresoraUseCase _registerDiscovered;
    private readonly GeneralSettingsStore _settings;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isSaving;
    [ObservableProperty] private bool _isDiscovering;
    [ObservableProperty] private bool _mostrandoDescubrimiento;
    [ObservableProperty] private bool _isEditing;
    [ObservableProperty] private int _editingId;
    [ObservableProperty] private string? _mensaje;
    [ObservableProperty] private bool _esError;
    [ObservableProperty] private string _subnet = string.Empty;
    [ObservableProperty] private bool _registrarComoDescubierta;

    // ===== Formulario =====
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private int _connectionType;
    [ObservableProperty] private string _address = string.Empty;
    [ObservableProperty] private int _port = 9100;
    [ObservableProperty] private bool _active = true;

    [ObservableProperty] private ImpresoraModel? _respaldoSeleccionado;

    public ObservableCollection<ImpresoraModel> Impresoras { get; } = new();
    public ObservableCollection<ImpresoraDescubiertaModel> Descubiertas { get; } = new();
    public ObservableCollection<ImpresoraModel> ImpresorasDisponiblesParaRespaldo { get; } = new();

    public bool TieneMensaje => !string.IsNullOrWhiteSpace(Mensaje);
    public bool MensajeExito => TieneMensaje && !EsError;
    public bool MensajeError => TieneMensaje && EsError;
    public bool HayImpresoras => Impresoras.Count > 0;
    public bool HayDescubiertas => Descubiertas.Count > 0;
    public string TituloFormulario => IsEditing ? "Editar impresora" : "Nueva impresora";

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

    partial void OnIsEditingChanged(bool value) => OnPropertyChanged(nameof(TituloFormulario));

    public PrintersComponentViewModel(
        GetImpresorasUseCase get,
        CreateImpresoraUseCase create,
        UpdateImpresoraUseCase update,
        DeleteImpresoraUseCase delete,
        DiscoverImpresorasUseCase discover,
        RegisterDiscoveredImpresoraUseCase registerDiscovered,
        GeneralSettingsStore settings)
    {
        _get = get;
        _create = create;
        _update = update;
        _delete = delete;
        _discover = discover;
        _registerDiscovered = registerDiscovered;
        _settings = settings;
        _port = settings.Current.DefaultPrinterPort;
        _ = CargarAsync();
    }

    [RelayCommand]
    private async Task Cargar() => await CargarAsync();

    private async Task CargarAsync()
    {
        try
        {
            IsLoading = true;
            Mensaje = null;
            EsError = false;

            var impresoras = await _get.Execute();
            Impresoras.Clear();
            foreach (var impresora in impresoras)
                Impresoras.Add(impresora);

            OnPropertyChanged(nameof(HayImpresoras));
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

    private void ActualizarOpcionesRespaldo()
    {
        ImpresorasDisponiblesParaRespaldo.Clear();
        foreach (var impresora in Impresoras.Where(i => i.Id != EditingId))
            ImpresorasDisponiblesParaRespaldo.Add(impresora);
    }

    [RelayCommand]
    private void Nuevo()
    {
        EditingId = 0;
        Name = string.Empty;
        ConnectionType = 0;
        Address = string.Empty;
        Port = _settings.Current.DefaultPrinterPort;
        Active = true;
        RegistrarComoDescubierta = false;
        ActualizarOpcionesRespaldo();
        RespaldoSeleccionado = null;
        Mensaje = null;
        EsError = false;
        IsEditing = true;
    }

    [RelayCommand]
    private void Editar(ImpresoraModel impresora)
    {
        if (impresora == null) return;

        EditingId = impresora.Id;
        Name = impresora.Name;
        ConnectionType = impresora.ConnectionType;
        Address = impresora.ConnectionType == 1 && impresora.Address.StartsWith("usb://")
            ? impresora.Address[6..]
            : impresora.Address;
        Port = impresora.Port;
        Active = impresora.Active;
        RegistrarComoDescubierta = false;

        ActualizarOpcionesRespaldo();
        RespaldoSeleccionado = ImpresorasDisponiblesParaRespaldo
            .FirstOrDefault(i => i.Id == impresora.BackupPrinterId);

        Mensaje = null;
        EsError = false;
        IsEditing = true;
    }

    [RelayCommand]
    private void MostrarDescubrimiento()
    {
        MostrandoDescubrimiento = !MostrandoDescubrimiento;
    }

    [RelayCommand]
    private async Task Descubrir()
    {
        try
        {
            IsDiscovering = true;
            Descubiertas.Clear();
            Mensaje = null;
            EsError = false;

            var encontradas = await _discover.Execute(string.IsNullOrWhiteSpace(Subnet) ? null : Subnet.Trim());
            foreach (var impresora in encontradas)
                Descubiertas.Add(impresora);

            OnPropertyChanged(nameof(HayDescubiertas));

            if (Descubiertas.Count == 0)
            {
                EsError = true;
                Mensaje = "No se encontraron impresoras. Verificá la red o ingresá un rango (ej: 192.168.1.0/24).";
            }
        }
        catch (Exception ex)
        {
            EsError = true;
            Mensaje = $"Error al buscar impresoras: {ex.Message}";
        }
        finally
        {
            IsDiscovering = false;
        }
    }

    [RelayCommand]
    private async Task UsarDescubierta(ImpresoraDescubiertaModel impresora)
    {
        if (impresora == null) return;

        if (impresora.IsRegistered && impresora.RegisteredPrinterId.HasValue)
        {
            var registrada = Impresoras.FirstOrDefault(i => i.Id == impresora.RegisteredPrinterId.Value);
            if (registrada != null)
            {
                Editar(registrada);
                return;
            }
        }

        EditingId = 0;
        Name = impresora.Name;
        ConnectionType = impresora.ConnectionType;
        Address = impresora.ConnectionType == 1 && impresora.Address.StartsWith("usb://")
            ? impresora.Address[6..]
            : impresora.Address;
        Port = impresora.ConnectionType == 1 ? _settings.Current.DefaultPrinterPort : impresora.Port;
        Active = true;
        RegistrarComoDescubierta = true;

        ActualizarOpcionesRespaldo();
        RespaldoSeleccionado = null;

        Mensaje = null;
        EsError = false;
        IsEditing = true;
        await Task.CompletedTask;
    }

    [RelayCommand]
    private void Cancelar()
    {
        IsEditing = false;
        Mensaje = null;
        EsError = false;
    }

    [RelayCommand]
    private async Task Guardar()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                EsError = true;
                Mensaje = "El nombre de la impresora es obligatorio.";
                return;
            }

            if (ConnectionType == 0 && string.IsNullOrWhiteSpace(Address))
            {
                EsError = true;
                Mensaje = "La dirección IP es obligatoria para impresoras de red.";
                return;
            }

            IsSaving = true;
            Mensaje = null;
            EsError = false;

            var request = new ImpresoraRequest
            {
                Name = Name,
                ConnectionType = ConnectionType,
                Address = ConnectionType == 1 ? $"usb://{Address}" : Address,
                Port = ConnectionType == 1 ? 0 : Port,
                Active = Active,
                BackupPrinterId = RespaldoSeleccionado?.Id
            };

            if (EditingId != 0)
                await _update.Execute(EditingId, request);
            else if (RegistrarComoDescubierta)
                await _registerDiscovered.Execute(request);
            else
                await _create.Execute(request);

            RegistrarComoDescubierta = false;
            IsEditing = false;
            await CargarAsync();
            Mensaje = "Impresora guardada correctamente.";
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

    [RelayCommand]
    private async Task Eliminar(ImpresoraModel impresora)
    {
        if (impresora == null) return;

        try
        {
            await _delete.Execute(impresora.Id);
            await CargarAsync();
            Mensaje = "Impresora eliminada.";
        }
        catch (Exception ex)
        {
            EsError = true;
            Mensaje = ex.Message;
        }
    }
}
