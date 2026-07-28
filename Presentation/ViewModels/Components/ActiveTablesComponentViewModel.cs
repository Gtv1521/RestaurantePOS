using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Application.UseCases.Session;
using MiComanderaApp.Core.Application.UseCases.Table;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;

namespace MiComanderaApp.ViewModels.Components;

public partial class ActiveTablesComponentViewModel : ViewModelBase
{
    private readonly IViewModelFactory _factory;
    private readonly GetAllTablesUseCase _allTables;
    private readonly GetSessionSave _userSesion;
    private readonly GetTablesOpenUseCase _tablesOpen;


    public ObservableCollection<TableViewModel> Tables { get; } = new();

    public ActiveTablesComponentViewModel(
        IViewModelFactory factory,
        GetAllTablesUseCase allTables,
        GetTablesOpenUseCase tablesOpen,
        GetSessionSave userSesion
        )
    {
        _factory = factory;
        _tablesOpen = tablesOpen;
        _userSesion = userSesion;
        _allTables = allTables;
        _ = LoadOpenTables();
    }

    private async Task LoadOpenTables()
    {
        try
        {
            var response = await _tablesOpen.Execute();

            if (response == null || !response.Any())
            {
                System.Console.WriteLine("⚠️ No se encontraron mesas asignadas.");
                return;
            }

            Tables.Clear();

            foreach (var table in response)
            {
                var tableVm = _factory.Create<TableViewModel>();
                tableVm.InicializeVenta(table);

                Tables.Add(tableVm);
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"🚨 Error al cargar las mesas: {ex.Message}");
        }
    }
}
