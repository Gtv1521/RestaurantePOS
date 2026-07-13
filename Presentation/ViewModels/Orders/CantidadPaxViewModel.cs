using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.ViewModels.Mesas;

namespace MiComanderaApp.ViewModels.Orders;

public partial class CantidadPaxViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IViewModelFactory _factory;


    [ObservableProperty]
    private int _mesa;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AceptarCommand))]
    private string _cantidadPax = string.Empty;

    public CantidadPaxViewModel(INavigationService navigationService, IViewModelFactory factory)
    {
        _navigationService = navigationService;
        _factory = factory;
    }

    public void Initialize(int table)
    {
        Mesa = table;
    }

    [RelayCommand]
    private void AddDigit(string digit)
    {
        if (CantidadPax.Length < 2) // Límite de 99 personas
        {
            CantidadPax += digit;
        }
    }

    [RelayCommand]
    private void DeleteDigit()
    {
        if (CantidadPax.Length > 0)
        {
            CantidadPax = CantidadPax.Substring(0, CantidadPax.Length - 1);
        }
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Vuelve a la vista anterior
        var vm = _factory.Create<TablesViewModel>();
        _navigationService.NavigateTo(vm);
    }

    [RelayCommand(CanExecute = nameof(CanAccept))]
    private void Aceptar()
    {
        // TODO: Navegar a la vista de toma de pedido, pasando la mesa y la cantidad de personas.
        // Por ejemplo: _navigationService.NavigateTo<NewOrderViewModel>(new { Mesa, CantidadPax });
        System.Console.WriteLine($"Aceptado: {CantidadPax} personas para la mesa {Mesa}");
        var vm = _factory.Create<DataTableViewModel>();
        vm.Initialize(Mesa, int.Parse(CantidadPax));
        _navigationService.NavigateTo<DataTableViewModel>();
    }

    private bool CanAccept() => !string.IsNullOrEmpty(CantidadPax) && CantidadPax != "0";
}
