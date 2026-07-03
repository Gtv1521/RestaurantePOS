using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Models;

namespace MiComanderaApp.ViewModels;

public partial class TableViewModel : ViewModelBase
{
    private readonly TableModel _table;

    public int TableNumber => _table.TableNumber;
    public TableStatus Status => _table.Status;

    public TableViewModel(TableModel table)
    {
        _table = table;
    }

    public static readonly IValueConverter StatusToClassConverter =
        new FuncValueConverter<TableStatus, string?>(status => status switch
        {
            TableStatus.Disponible => "available",
            TableStatus.Ocupada => "occupied",
            TableStatus.Reservada => "reserved",
            _ => null
        });
}

public class FuncValueConverter<TIn, TOut> : IValueConverter
{
    private readonly Func<TIn, TOut> _converter;
    public FuncValueConverter(Func<TIn, TOut> converter) => _converter = converter;
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is TIn val ? _converter(val) : default;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}