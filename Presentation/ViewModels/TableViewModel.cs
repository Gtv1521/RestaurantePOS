using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
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
    private readonly OcuparTableUseCase _ocuparMesa;
    private readonly TableState _dataTable;

    public TableViewModel(
        INavigationService navigationService,
        IViewModelFactory factory,
        OcuparTableUseCase ocuparMesa,
        TableState dataTable)
    {
        _navigationService = navigationService;
        _dataTable = dataTable;
        _ocuparMesa = ocuparMesa;
        _factory = factory;
    }

    [NotifyPropertyChangedFor(nameof(ColorEstado))]
    [ObservableProperty] private string _status = "Libre";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TiempoEstimado))]

    private DateTime? _horaApertura;
    [ObservableProperty] private int _tableNumber;
    [ObservableProperty] private TableModel? _table;
    [ObservableProperty] private VentaModel? _venta;
    [ObservableProperty] private bool _open = false;
    [ObservableProperty] private bool _vacio = false;
    [ObservableProperty] private bool _pax = false;
    [ObservableProperty] private int? _instancia = 0;
    [ObservableProperty] private string? _cliente;
    [ObservableProperty] private int? _cantidadPax;
    [ObservableProperty] private string _claseEstado = "";

    public string ColorEstado => Status switch
    {
        "Libre" => "#4CAF50",
        "Ocupado" => "#af5d40",
        "Reservado" => "#ED8936",
        "Disponible" => "#34AA55",
        _ => "#95A5A6"
    };

    public string TiempoTranscurrido
    {
        get
        {
            if (!HoraApertura.HasValue)
                return "⏱️ 0s";

            var tiempo = DateTime.Now - HoraApertura.Value;

            return tiempo.TotalSeconds switch
            {
                < 60 => $"⏱️{tiempo.Seconds}s",
                < 3600 => $"⏱️{tiempo.Minutes} {(tiempo.Minutes == 1 ? "m" : "ms")}",
                < 86400 => $"⏱️{tiempo.Hours}h {tiempo.Minutes}m",
                _ => $"⏱️{tiempo.Days}d {tiempo.Hours}h {tiempo.Minutes}m"
            };
        }
    }

    public string TiempoEstimado
    {
        get
        {
            if (!HoraApertura.HasValue)
                return "#9E9E9E";

            var minutos = (DateTime.Now - HoraApertura.Value).Minutes;

            return minutos switch
            {
                <= 10 => "#4CAF50",
                <= 20 => "#FFC107",
                <= 30 => "#FF9800",
                <= 40 => "#FF5722",
                _ => "#F44336"
            };
        }
    }

    public void Initialize(TableModel table)
    {
        Table = table;
        Status = table.Estado;
        TableNumber = table.NumeroMesa;

        if (Status == "Ocupado")
        {
            Pax = true;
            Open = true;
            Instancia = table.VentasActivas.Count();
            CantidadPax = table.VentasActivas.FirstOrDefault()?.Pax ?? 0;
        }
        else if (Status == "Libre")
        {
            Vacio = true;
            Open = false;
            Pax = false;
        }
        else if (Status == "Reservado")
        {
            Open = true;
            Pax = false;
        }

    }

    public void InicializeVenta(VentaModel venta)
    {
        Venta = venta;
        CantidadPax = venta.Pax;
        Cliente = venta.Alias;
        Status = "Ocupado";
        TableNumber = venta.NumeroMesa;
        HoraApertura = venta.FechaApertura.ToLocalTime();
    }


    [RelayCommand]
    public async Task OpenTable()
    {
        _dataTable.DataTable = Table;

        switch (Status)
        {
            case "Ocupada":
                // Si la mesa está ocupada, vamos directo a la vista de la comanda.
                var dataTableVm = _factory.Create<DataTableViewModel>();
                // Aquí asumimos que VentaModel se obtiene de otra parte,
                // por ahora se inicializa con valores del TableModel.
                var venta = new VentaModel { NumeroMesa = Table!.NumeroMesa, Instancia = 0 }; // Ejemplo
                dataTableVm.Initialize(venta, Table.Capacidad);
                _navigationService.NavigateTo(dataTableVm);
                break;

            case "Libre":
                var ventas = await _ocuparMesa.Execute(Table!.Id);
                var cantidadPaxVm = _factory.Create<CantidadPaxViewModel>();
                _ = cantidadPaxVm.Initialize(TableNumber, ventas);
                _navigationService.NavigateTo(cantidadPaxVm);
                break;

            case "Reservada":
                // Lógica para mesas reservadas (puedes implementarla aquí).
                System.Diagnostics.Debug.WriteLine($"La mesa {TableNumber} está reservada.");
                break;
        }
    }

    [RelayCommand]
    private void SelectTable(VentaModel venta)
    {

    }
}



public class FuncValueConverter<TIn, TOut> : IValueConverter
{
    private readonly Func<TIn, TOut> _converter;
    public FuncValueConverter(Func<TIn, TOut> converter) => _converter = converter;
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is TIn val ? _converter(val) : default;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}