using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.UseCases.Table;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.ViewModels.Mesas;

namespace MiComanderaApp.ViewModels.Orders;

public partial class CantidadPaxViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IViewModelFactory _factory;
    private readonly OcuparTableUseCase _ocuparCase;



    [ObservableProperty] private int _mesa;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AceptarCommand))]
    private string _cantidadPax = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Title))]
    private bool _newMesa = false;
    private VentaModel _dataVenta = new();

    public CantidadPaxViewModel(
        INavigationService navigationService,
        IViewModelFactory factory,
        OcuparTableUseCase ocuparCase)
    {
        _navigationService = navigationService;
        _ocuparCase = ocuparCase;
        _factory = factory;
    }

    public async Task Initialize(int table, VentaModel venta)
    {
        Mesa = table;
        System.Console.WriteLine(venta.VentaId);
        await State(true);
    }
    public async Task State(bool openTable)
    {
        NewMesa = openTable;
    }

    private async Task<VentaModel> OcuparMesa(int id)
    {
        return await _ocuparCase.Execute(id);
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
            vm.Initialize(int.Parse(CantidadPax), await OcuparMesa(NewMesa););
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
