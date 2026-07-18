using System;
using System.Runtime.InteropServices;
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


    [ObservableProperty] private int _mesa;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AceptarCommand))]
    private string _cantidadPax = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Title))]
    private bool _newMesa = false;

    public CantidadPaxViewModel(INavigationService navigationService, IViewModelFactory factory)
    {
        _navigationService = navigationService;
        _factory = factory;
    }

    public void Initialize(int table)
    {
        Mesa = table;
    }
    public void State(bool openTable)
    {
        NewMesa = openTable;
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
        CantidadPax = string.Empty;
    }

    [RelayCommand(CanExecute = nameof(CanAccept))]
    private void Aceptar()
    {
        if (!NewMesa)
        {
            var vm = _factory.Create<CantidadPaxViewModel>();
            vm.Initialize(int.Parse(CantidadPax));
            vm.State(true);
            _navigationService.NavigateTo(vm);
            CantidadPax = string.Empty;

        }
        else
        {
            var vm = _factory.Create<DataTableViewModel>();
            vm.Initialize(Mesa, int.Parse(CantidadPax));
            _navigationService.NavigateTo(vm);
            CantidadPax = string.Empty;

        }

    }

    private bool CanAccept() => !string.IsNullOrEmpty(CantidadPax) && CantidadPax != "0";
    public string Title => NewMesa == false ? "Digite numero de mesa" : "Digite cantidad de pax";
}
