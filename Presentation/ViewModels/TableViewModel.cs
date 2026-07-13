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
using MiComanderaApp.ViewModels.Mesas;
using MiComanderaApp.ViewModels.Orders;

namespace MiComanderaApp.ViewModels;

public partial class TableViewModel : ViewModelBase
{
    [ObservableProperty] private TableStatus _status;
    [ObservableProperty] private int _tableNumber;
    private TableModel? _table;
    private readonly INavigationService _navigationService;
    private readonly IViewModelFactory _factory;

    public TableViewModel(INavigationService navigationService, IViewModelFactory factory)
    {
        _navigationService = navigationService;
        _factory = factory;
    }

    public void Initialize(TableModel table)
    {
        _table = table;
        Status = table.Status;
        TableNumber = table.TableNumber;
    }

    public static readonly IValueConverter StatusToClassConverter =
        new FuncValueConverter<TableStatus, string?>(status => status switch
        {
            TableStatus.Disponible => "available",
            TableStatus.Ocupada => "occupied",
            TableStatus.Reservada => "reserved",
            _ => null
        });

    [RelayCommand]
    public void OpenTable()
    {
        if (Status == TableStatus.Ocupada)
        {
            var vm = _factory.Create<DataTableViewModel>();
            vm.Initialize(TableNumber, 2);
            _navigationService.NavigateTo(vm);
        }
        else
        {
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