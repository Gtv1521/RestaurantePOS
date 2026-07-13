using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;

namespace MiComanderaApp.ViewModels.Components;

public partial class AllTablesComponentViewModel : ViewModelBase
{
    private readonly IViewModelFactory _factory;
    public ObservableCollection<TableViewModel> Tables { get; } = new();
    public AllTablesComponentViewModel(IViewModelFactory factory)
    {
        _factory = factory;

        LoadTables();
    }

    public void LoadTables()
    {
        var openTableNumbers = new List<int> { 1, 2, 3, 4, 8, 11, 20, 23, 40 };
        var allTables = Enumerable.Range(1, 80).Select(i => new TableModel
        {
            TableNumber = i,
            Status = openTableNumbers.Contains(i) ? TableStatus.Ocupada : TableStatus.Disponible
        }).ToList();

        foreach (var table in allTables)
        {
            var tableViewModel = _factory.Create<TableViewModel>();
            tableViewModel.Initialize(table);
            Tables.Add(tableViewModel);
        }
    }
}
