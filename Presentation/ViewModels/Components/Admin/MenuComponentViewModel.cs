using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.ViewModels.Components.Products;

namespace MiComanderaApp.ViewModels.Components.Admin;

public partial class MenuComponentViewModel : ViewModelBase
{
    private readonly IViewModelFactory _factory;

    public ObservableCollection<ProductoModel> Products { get; } = new();

    public MenuComponentViewModel(IViewModelFactory factory)
    {
        _factory = factory;
        LoadProducts();
    }


    [RelayCommand]
    private void AddNewItem()
    {
        System.Console.WriteLine("Agregar nuevo item");
    }


    public void LoadProducts()
    {
        var products = new List<ProductoModel>
        {
            new ProductoModel { Name = "Papas", PrinterName = "Hola", Price = 200 },
            new ProductoModel {  Name = "Patatas", PrinterName = "Patatas", Price = 200 }
        };


        // Filtrar solo las mesas activas (Ocupadas o Reservadas)
        foreach (var table in products)
        {
            Products.Add(table);
        }

    }



}
