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
    private readonly GetAllTablesUseCase _allTbles;
    private readonly GetSessionSave _userSesion;


    public ObservableCollection<TableViewModel> Tables { get; } = new();

    public ActiveTablesComponentViewModel(
        IViewModelFactory factory,
        GetAllTablesUseCase allTbles,
        GetSessionSave userSesion
        )
    {
        _factory = factory;
        _userSesion = userSesion;
        _allTbles = allTbles;
        _ = LoadAllTables();
    }

    private async Task LoadAllTables()
    {
        try
        {
            var response = await _allTbles.Execute();

            if (response == null || !response.Any())
            {
                System.Console.WriteLine("⚠️ No se encontraron mesas asignadas.");
                return;
            }

            System.Console.WriteLine($"✅ Cantidad de mesas recibidas: {response.Count()}");

            // 4. Limpiamos la lista actual antes de recargar (evita duplicar elementos en la interfaz)
            Tables.Clear();

            // 5. Instanciamos e inicializamos cada View Model
            foreach (var table in response)
            {
                var tableVm = _factory.Create<TableViewModel>();
                tableVm.Initialize(table);

                Tables.Add(tableVm);
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"🚨 Error al cargar las mesas: {ex.Message}");
        }
    }

    private async Task LoadTablesMesero(string id)
    {
        
    }
}
