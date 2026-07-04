using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.ViewModels.Orders;

namespace MiComanderaApp.ViewModels;

public partial class TablesViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _viewTitle = "Mesas Abiertas";

    public ObservableCollection<TableViewModel> Tables { get; } = new();

    public TablesViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        // Datos de ejemplo. Esto debería venir de un servicio o base de datos.
        var allTables = new[]
        {
            new TableModel { TableNumber = 1, Status = TableStatus.Ocupada },
            new TableModel { TableNumber = 2, Status = TableStatus.Ocupada },
            new TableModel { TableNumber = 3, Status = TableStatus.Ocupada },
            new TableModel { TableNumber = 4, Status = TableStatus.Reservada },
            new TableModel { TableNumber = 5, Status = TableStatus.Ocupada },
            new TableModel { TableNumber = 6, Status = TableStatus.Ocupada },
            new TableModel { TableNumber = 7, Status = TableStatus.Ocupada },
            new TableModel { TableNumber = 8, Status = TableStatus.Disponible },
            new TableModel { TableNumber = 9, Status = TableStatus.Disponible },
            new TableModel { TableNumber = 10, Status = TableStatus.Disponible },
            new TableModel { TableNumber = 11, Status = TableStatus.Ocupada },
            new TableModel { TableNumber = 12, Status = TableStatus.Disponible },
            new TableModel { TableNumber = 13, Status = TableStatus.Ocupada },
        };

        // Cargar inicialmente solo las mesas abiertas (ocupadas)
        foreach (var table in allTables.Where(t => t.Status == TableStatus.Ocupada))
        {
            Tables.Add(new TableViewModel(table));
        }
    }

    [RelayCommand]
    public async Task AbirNuevaOrden()
    {
        // Aquí puedes implementar la lógica para abrir una nueva orden.
        // Por ejemplo, podrías navegar a otra vista o mostrar un diálogo.
        System.Console.WriteLine("Abriendo nueva orden...");
        // await Task.Delay(200); // Simula un retraso
        _navigationService.NavigateTo<NewOrderViewModel>();
    }

    [RelayCommand]
    public void Salir()
    {
        // Aquí puedes implementar la lógica para salir de la aplicación.
        System.Console.WriteLine("Saliendo de la aplicación...");
        _navigationService.NavigateTo<LoginViewModel>();
    }
}