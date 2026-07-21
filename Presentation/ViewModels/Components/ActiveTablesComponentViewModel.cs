using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Application.UseCases.Session;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;

namespace MiComanderaApp.ViewModels.Components;

public partial class ActiveTablesComponentViewModel : ViewModelBase
{
    private readonly IViewModelFactory _factory;
    private readonly ISingleCrud<TableModel, TableRequest> _allTbles;
    private readonly GetSessionSave _userSesion;


    public ObservableCollection<TableModel> Tables { get; } = new();

    public ActiveTablesComponentViewModel(
        IViewModelFactory factory,
        ISingleCrud<TableModel, TableRequest> allTbles,
        GetSessionSave userSesion
        )
    {
        _factory = factory;
        _userSesion = userSesion;
        _allTbles = allTbles;
        _ = LoadTables();
    }

    private async Task LoadTables()
    {
        var response = await _allTbles.GetAllAsync(_userSesion.Execute().UserId.ToString(), 1, 40);
        foreach (var table in response)
        {
            Tables.Add(table);
        }
    }
}
