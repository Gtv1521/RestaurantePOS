using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;

namespace MiComanderaApp.ViewModels.Components;

public partial class ActiveTablesComponentViewModel : ViewModelBase
{
    private readonly IViewModelFactory _factory;
    public ObservableCollection<TableViewModel> Tables { get; } = new();

    public ActiveTablesComponentViewModel(IViewModelFactory factory)
    {
        _factory = factory;
        // Simulación de datos de ejemplo
        var allTables = new List<TableModel>
        {
            new TableModel { TableNumber = 1, Status = TableStatus.Ocupada },
            new TableModel { TableNumber = 2, Status = TableStatus.Disponible },
            new TableModel { TableNumber = 3, Status = TableStatus.Ocupada },
            new TableModel { TableNumber = 4, Status = TableStatus.Ocupada },
            new TableModel { TableNumber = 5, Status = TableStatus.Ocupada }
        };

        // Filtrar solo las mesas activas (Ocupadas o Reservadas)
        foreach (var table in allTables)
        {
            if (table.Status == TableStatus.Ocupada || table.Status == TableStatus.Reservada)
            {
                var tableVm = _factory.Create<TableViewModel>();
                tableVm.Initialize(table);

                Tables.Add(tableVm);
                // 🚀 SOLUCIÓN: Le pasamos 'table' directamente entre los paréntesis
            }
        }
    }
}
