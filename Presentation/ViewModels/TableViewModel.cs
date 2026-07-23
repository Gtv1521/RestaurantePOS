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

    [ObservableProperty] private string _status = "Free";
    [ObservableProperty] private int _tableNumber;
    [ObservableProperty] private TableModel? _table;
    [ObservableProperty] private string _colorEstado = "";
    [ObservableProperty] private bool _pax = false;
    [ObservableProperty] private bool _open = false;
    [ObservableProperty] private bool _vacio = false;
    [ObservableProperty] private string _claseEstado = "";


    public void Initialize(TableModel table)
    {
        Table = table;
        Status = table.Estado;
        TableNumber = table.NumeroMesa;
        ColorEstado = table.estadoColor;

        if (Table.Estado == "Ocupado") Pax = true;
        if (Table.VentasActivas.Count() > 0) Open = true;
        if (Table.VentasActivas.Count() == 0) Vacio = true;
        System.Console.WriteLine(table.Id);
    }

    [RelayCommand]
    public async Task OpenTable()
    {
        if (Status == "Ocupado")
        {
            if (Table?.VentasActivas.Count() > 1)
            { System.Console.WriteLine("Ir a otro panel"); }
            else
            {
                var venta = await OcuparMesa(Table?.Id ?? 0);
                _dataTable.DataTable = Table;
                var vm = _factory.Create<DataTableViewModel>();
                vm.Initialize(TableNumber, Table?.Capacidad ?? 0, venta);
                _navigationService.NavigateTo(vm);
            }
        }
        else
        {
            _dataTable.DataTable = Table;
            var vm = _factory.Create<CantidadPaxViewModel>();
            vm.Initialize(TableNumber);
            _navigationService.NavigateTo(vm);
        }
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