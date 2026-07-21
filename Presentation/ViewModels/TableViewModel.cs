using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using Avalonia.Data.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    public TableViewModel(INavigationService navigationService, IViewModelFactory factory, TableState dataTable)
    {
        _navigationService = navigationService;
        _dataTable = dataTable;
        _factory = factory;
    }

    [ObservableProperty] private string _status = "Free";
    [ObservableProperty] private int _tableNumber;
    [ObservableProperty] private TableModel? _table;
    [ObservableProperty] private string _claseEstado = "";


    public void Initialize(TableModel table)
    {
        Table = table;
        Status = table.Estado;
        TableNumber = table.NumeroMesa;
    }

    // public static readonly IValueConverter StatusToClassConverter =
    //     new FuncValueConverter<TableStatus, string?>(status => status switch
    //     {
    //         TableStatus.Disponible => "available",
    //         TableStatus.Ocupada => "occupied",
    //         TableStatus.Reservada => "reserved",
    //         _ => null
    //     });

    [RelayCommand]
    public void OpenTable()
    {
        if (Status == Table?.Estado)
        {
            _dataTable.DataTable = Table;
            var vm = _factory.Create<DataTableViewModel>();
            vm.Initialize(TableNumber, Table?.Capacidad ?? 0);
            _navigationService.NavigateTo(vm);
        }
        else
        {
            _dataTable.DataTable = Table;
            var vm = _factory.Create<CantidadPaxViewModel>();
            vm.Initialize(TableNumber);
            _navigationService.NavigateTo(vm);
        }
    }
}

public class FuncValueConverter<TIn, TOut> : IValueConverter
{
    private readonly Func<TIn, TOut> _converter;
    public FuncValueConverter(Func<TIn, TOut> converter) => _converter = converter;
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is TIn val ? _converter(val) : default;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}