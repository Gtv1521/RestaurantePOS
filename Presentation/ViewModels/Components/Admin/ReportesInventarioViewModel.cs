using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.UseCases.Inventario;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.ViewModels;

namespace MiComanderaApp.ViewModels.Components.Admin;

public partial class ReportesInventarioViewModel : ViewModelBase
{
    private readonly GetReporteInventarioUseCase _getReporte;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string? _mensajeError;
    [ObservableProperty] private InventarioReporteModel? _reporte;

    public ObservableCollection<IngredienteCriticoModel> Criticos { get; } = new();
    public ObservableCollection<CostoProductoModel> CostosProductos { get; } = new();
    public ObservableCollection<MovimientoInventarioModel> Movimientos { get; } = new();

    public ReportesInventarioViewModel(GetReporteInventarioUseCase getReporte)
    {
        _getReporte = getReporte;
        _ = CargarReporte();
    }

    [RelayCommand]
    private async Task CargarReporte()
    {
        IsLoading = true;
        MensajeError = null;
        try
        {
            var data = await _getReporte.Execute();
            if (data == null) return;

            Reporte = data;

            Criticos.Clear();
            foreach (var c in data.IngredientesCriticos) Criticos.Add(c);

            CostosProductos.Clear();
            foreach (var p in data.CostosProductos.OrderByDescending(p => p.MargenGanancia)) CostosProductos.Add(p);

            Movimientos.Clear();
            foreach (var m in data.MovimientosRecientes) Movimientos.Add(m);
        }
        catch (Exception ex)
        {
            MensajeError = $"No se pudo cargar el reporte: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public string ValorTotal => $"S/ {(Reporte?.Resumen.ValorTotalInventario ?? 0):N2}";
    public string CostoPromedio => $"S/ {(Reporte?.Resumen.CostoPromedioProducto ?? 0):N2}";
    public string MargenPromedio => $"{(Reporte?.Resumen.MargenGananciaPromedio ?? 0):N1}%";

    partial void OnReporteChanged(InventarioReporteModel? value)
    {
        OnPropertyChanged(nameof(ValorTotal));
        OnPropertyChanged(nameof(CostoPromedio));
        OnPropertyChanged(nameof(MargenPromedio));
    }
}
