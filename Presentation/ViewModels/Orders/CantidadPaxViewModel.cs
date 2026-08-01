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
    [ObservableProperty] private int _instancia;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AceptarCommand))]
    private string _cantidadPax = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Title))]
    private bool _newMesa = false;
    [ObservableProperty] private VentaModel _dataVenta = new();

    public CantidadPaxViewModel(
        INavigationService navigationService,
        IViewModelFactory factory,
        OcuparTableUseCase ocuparCase)
    {
        _navigationService = navigationService;
        _ocuparCase = ocuparCase;
        _factory = factory;
    }

    public async Task Initialize(int table, VentaModel? ventaModel)
    {
        Mesa = table;
        DataVenta = ventaModel!;
        Instancia = ventaModel!.Instancia;
        State(true);
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
    private async Task Aceptar()
    {
        if (!NewMesa) // abre la mesa 
        {
            var venta = await OcuparMesa(int.Parse(CantidadPax));
            Instancia = venta.Instancia;
            var vm = _factory.Create<CantidadPaxViewModel>();
            await vm.Initialize(int.Parse(CantidadPax), venta);
            vm.State(true);
            _navigationService.NavigateTo(vm);

        }
        else // edita la cantidad de personas 
        {
            var vm = _factory.Create<DataTableViewModel>();
            vm.Initialize(DataVenta, int.Parse(CantidadPax));
            _navigationService.NavigateTo(vm);

        }
        CantidadPax = string.Empty;
    }

    private bool CanAccept() => !string.IsNullOrEmpty(CantidadPax) && CantidadPax != "0";
}
