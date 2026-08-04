using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.UseCases.Table;
using MiComanderaApp.Core.Application.UseCases.Venta;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.ViewModels.Mesas;

namespace MiComanderaApp.ViewModels.Orders;

public partial class CantidadPaxViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IViewModelFactory _factory;
    private readonly OcuparTableUseCase _ocuparCase;
    private readonly UpdatePaxUseCase _updatePaxCase;


    [ObservableProperty] private int _mesa;
    [ObservableProperty] private int _instancia;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AceptarCommand))]
    private string _cantidadPax = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Title))]
    private bool _newMesa = false;
    [ObservableProperty] private VentaModel? _dataVenta;

    public CantidadPaxViewModel(
        INavigationService navigationService,
        IViewModelFactory factory,
        UpdatePaxUseCase updatePaxCase,
        OcuparTableUseCase ocuparCase)
    {
        _navigationService = navigationService;
        _ocuparCase = ocuparCase;
        _updatePaxCase = updatePaxCase;
        _factory = factory;
    }

    public async Task Initialize(int table, VentaModel? ventaModel)
    {
        Mesa = table;
        DataVenta = ventaModel!;
        Instancia = ventaModel!.Instancia;
        State(true);
    }

    public async Task InitializeVenta(VentaModel ventaModel)
    {
        Mesa = ventaModel.NumeroMesa;
        DataVenta = ventaModel;
        Instancia = ventaModel.Instancia;
    }

    public void State(bool openTable)
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
    private async Task Aceptar()
    {
        if (!NewMesa) // abre la mesa 
        {
            await OpenTableAndNavigateAsync();
        }
        else // edita la cantidad de personas 
        {
            await SaveDataVentaAsync();

        }
        CantidadPax = string.Empty;
    }

    // ocupar mesa y navegar a la vista de cantidad de pax
    private async Task OpenTableAndNavigateAsync()
    {
        try
        {
            var venta = await OcuparMesa(int.Parse(CantidadPax));
            var vm = _factory.Create<CantidadPaxViewModel>();
            await vm.InitializeVenta(venta);
            vm.State(true);
            _navigationService.NavigateTo(vm);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error al ocupar la mesa: {ex.Message}");
        }
    }

    // valida y actualiza el numero de personas
    private async Task SaveDataVentaAsync()
    {
        try
        {
            var result = await _updatePaxCase.ExecuteAsync(DataVenta!.VentaId, int.Parse(CantidadPax));
            if (!result) System.Console.WriteLine("Error al actualizar la cantidad de pax");
            GoToDataTable();
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error al guardar la cantidad de pax: {ex.Message}");
        }
    }
    // cambiar de vista a data table
    private void GoToDataTable()
    {
        var vm = _factory.Create<DataTableViewModel>();
        vm.Initialize(DataVenta!, int.Parse(CantidadPax));
        _navigationService.NavigateTo(vm);
    }

    private bool CanAccept() => !string.IsNullOrEmpty(CantidadPax) && CantidadPax != "0";
    public string Title => NewMesa == false ? "Digite numero de mesa" : "Digite cantidad de pax";
}
