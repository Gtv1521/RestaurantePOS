using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using MiComanderaApp.Core.Application.UseCases.Table;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;

namespace MiComanderaApp.ViewModels.Components;

public partial class AllTablesComponentViewModel : ViewModelBase
{
    private readonly IViewModelFactory _factory;
    private readonly GetAllTablesUseCase _allTables;


    public ObservableCollection<TableViewModel> Tables { get; } = new();
    public AllTablesComponentViewModel(
        IViewModelFactory factory,
        GetAllTablesUseCase allTables
        )
    {
        _factory = factory;
        _allTables = allTables;
        _ = LoadAllTables();
    }
    private async Task LoadAllTables()
    {
        var response = await _allTables.Execute();
        if (response == null || !response.Any())
        {
            System.Console.WriteLine("⚠️ No se encontraron mesas asignadas.");
            return;
        }
        foreach (var table in response)
        {
            var tableVm = _factory.Create<TableViewModel>();
            tableVm.Initialize(table);
            Tables.Add(tableVm);
        }
    }
}
