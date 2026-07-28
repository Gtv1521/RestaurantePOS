using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Data.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.UseCases.Table;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.Presentation.States;
using MiComanderaApp.ViewModels.Mesas;
using MiComanderaApp.ViewModels.Orders;

namespace MiComanderaApp.ViewModels;

public partial class TableViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IViewModelFactory _factory;
    private readonly TableState _dataTable;
    private readonly OcuparTableUseCase _ocuparCase;

    public TableViewModel(
        INavigationService navigationService,
        IViewModelFactory factory,
        OcuparTableUseCase ocuparCase,
        TableState dataTable)
    {
        _navigationService = navigationService;
        _dataTable = dataTable;
        _ocuparCase = ocuparCase;
        _factory = factory;
    }

    // [ObservableProperty] private string _status = "Free";
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ColorEstado))]
    // [NotifyPropertyChangedFor(nameof(TiempoEstimado))]
    private string _status = "Libre";
    [ObservableProperty] private int _tableNumber;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TiempoEstimado))]
    [NotifyPropertyChangedFor(nameof(TiempoTranscurrido))]
    private DateTime? _horaApertura;
    [ObservableProperty] private TableModel _table = new();
    [ObservableProperty] private VentaModel _venta = new();
    [ObservableProperty] private bool _pax = false;
    [ObservableProperty] private bool _open = false;
    [ObservableProperty] private bool _vacio = false;
    [ObservableProperty] private int _cantidadPax = 0;
    [ObservableProperty] private string? _cliente;
    [ObservableProperty] private string _claseEstado = "";
    public string ColorEstado => Status switch
    {
        "Ocupado" => "#be624b", // Un color ámbar para "Ocupado"
        "Reservado" => "#6ebd6a", // Un color azul claro para "Reservado"
        "Libre" => "#4CAF50", // Un color verde para "Libre"
        _ => "#9E9E9E" // Un color gris por defecto para cualquier otro estado
    };

    public string TiempoEstimado
    {
        get
        {
            if (!HoraApertura.HasValue)
                return "#2d35a4";

            var minutos = (DateTime.Now - HoraApertura.Value).Minutes;

            return minutos switch
            {
                <= 10 => "#4CAF50",
                <= 20 => "#bcc154",
                <= 30 => "#e2a140",
                <= 40 => "#FF5722",
                _ => "#F44336"
            };
        }
    }

    public string TiempoTranscurrido
    {
        get
        {
            if (!HoraApertura.HasValue)
                return "⏱️ 0s";

            var tiempo = DateTime.Now - HoraApertura.Value;

            return tiempo.TotalSeconds switch
            {
                < 60 => $"⏱️ {tiempo.Seconds}s",
                < 3600 => $"⏱️{tiempo.Minutes} {(tiempo.Minutes == 1 ? "m" : "ms")}",
                < 86400 => $"⏱️{tiempo.Hours}h {tiempo.Minutes}m",
                _ => $"⏱️{tiempo.Days}d {tiempo.Hours}h {tiempo.Minutes}m"
            };
        }

    }

    public void Initialize(TableModel table)
    {
        Table = table;
        Status = table?.Estado!;
        TableNumber = table?.NumeroMesa ?? 0;

        if (Table?.Estado == "Ocupado") Pax = true;
        if (Table?.VentasActivas.Count() > 0) Open = true;
        if (Table?.VentasActivas.Count() == 0) Vacio = true;
    }

    public void InicializeVenta(VentaModel venta)
    {
        HoraApertura = venta.FechaApertura.ToLocalTime();
        TableNumber = venta.NumeroMesa;
        Venta = venta;
        CantidadPax = venta.Pax;
        Status = "Ocupado";
        Cliente = venta.Alias;
    }


    [RelayCommand]
    public async Task OpenTable()
    {
        System.Console.WriteLine(TiempoEstimado);
        if (Status == "Ocupado")
        {
            if (Table?.VentasActivas.Count() > 1) // si tiene mas de una venta
            {
                var opens = Table?.VentasActivas;
                foreach (var item in opens!)
                {
                    System.Console.WriteLine($"Venta: {item.VentaId} ");
                }
            }
            else // si tiene una venta 
            {
                _dataTable.DataTable = Table;
                var vm = _factory.Create<DataTableViewModel>();
                vm.Initialize(Table?.VentasActivas.First()!, Table?.Capacidad ?? 0);
                _navigationService.NavigateTo(vm);
            }
        }
        else // si no tiene ventas 
        {
            var venta = await OcuparMesa(TableNumber); // se cre instancia de la mesa 
            _dataTable.DataTable = Table;
            var vm = _factory.Create<CantidadPaxViewModel>();
            await vm.Initialize(TableNumber, venta);
            _navigationService.NavigateTo(vm);
        }
    }

    [RelayCommand]
    private void SelectTable()
    {
        _dataTable.DataTable = Table;
        var vm = _factory.Create<DataTableViewModel>();
        vm.Initialize(Venta, Table?.Capacidad ?? 0);
        _navigationService.NavigateTo(vm);
    }

    private async Task<VentaModel> OcuparMesa(int id)
    {
        return await _ocuparCase.Execute(id);
    }
}


public class FuncValueConverter<TIn, TOut> : IValueConverter
{
    private readonly Func<TIn, TOut> _converter;
    public FuncValueConverter(Func<TIn, TOut> converter) => _converter = converter;
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is TIn val ? _converter(val) : default;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}


