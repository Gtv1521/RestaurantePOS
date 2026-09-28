using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Models;
using MiComanderaApp.ViewModels;

namespace MiComanderaApp.ViewModels.Components.Admin;

public partial class ReportesComponentViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _selectedReportType = "Diario";

    [ObservableProperty]
    private DateTime _selectedDate = DateTime.Today;

    [ObservableProperty]
    private DateTime _startDate = DateTime.Today.AddDays(-30);

    [ObservableProperty]
    private DateTime _endDate = DateTime.Today;

    [ObservableProperty]
    private bool _isLoading = false;

    public ObservableCollection<string> ReportTypes { get; } = new()
    {
        "Diario",
        "Purga de Sistema",
        "Balances Mensuales",
        "Balance de Devoluciones"
    };

    // Daily Report Properties
    [ObservableProperty]
    private decimal _totalVentasDiarias = 0;

    [ObservableProperty]
    private int _totalOrdenesDiarias = 0;

    [ObservableProperty]
    private int _totalMesasAtendidas = 0;

    [ObservableProperty]
    private decimal _ticketPromedioDiario = 0;

    [ObservableProperty]
    private ObservableCollection<DetalleVentaDiaria> _detalleVentasDiarias = new();

    // System Purge Properties
    [ObservableProperty]
    private int _registrosAntiguos = 0;

    [ObservableProperty]
    private long _espacioLiberadoMB = 0;

    [ObservableProperty]
    private ObservableCollection<LogPurga> _historialPurgas = new();

    // Monthly Balances Properties
    [ObservableProperty]
    private ObservableCollection<BalanceMes> _balancesMensuales = new();

    [ObservableProperty]
    private decimal _totalIngresosMes = 0;

    [ObservableProperty]
    private decimal _totalEgresosMes = 0;

    [ObservableProperty]
    private decimal _balanceNetoMes = 0;

    // Returns Balance Properties
    [ObservableProperty]
    private ObservableCollection<DevolucionItem> _devoluciones = new();

    [ObservableProperty]
    private int _totalDevoluciones = 0;

    [ObservableProperty]
    private decimal _montoTotalDevoluciones = 0;

    [ObservableProperty]
    private string _motivoFrecuente = "N/A";

    public ReportesComponentViewModel()
    {
        _ = LoadReportData();
    }

    // Computed KPI Collections for the View
    public IEnumerable<KpiCardModel> DailyKPIs => new[]
    {
        new KpiCardModel { Title = "VENTAS TOTALES", Value = $"S/ {TotalVentasDiarias:N2}", Subtitle = "Del día seleccionado" },
        new KpiCardModel { Title = "ÓRDENES", Value = TotalOrdenesDiarias.ToString(), Subtitle = "Órdenes completadas" },
        new KpiCardModel { Title = "MESAS ATENDIDAS", Value = TotalMesasAtendidas.ToString(), Subtitle = "Mesas con ventas" },
        new KpiCardModel { Title = "TICKET PROMEDIO", Value = $"S/ {TicketPromedioDiario:N2}", Subtitle = "Por orden" }
    };

    public IEnumerable<KpiCardModel> PurgeKPIs => new[]
    {
        new KpiCardModel { Title = "REGISTROS ANTIGUOS", Value = RegistrosAntiguos.ToString("N0"), Subtitle = "Pendientes de purga" },
        new KpiCardModel { Title = "ESPACIO A LIBERAR", Value = $"{EspacioLiberadoMB} MB", Subtitle = "En base de datos" },
        new KpiCardModel { Title = "ÚLTIMA PURGA", Value = HistorialPurgas.Count > 0 ? HistorialPurgas[0].Fecha.ToString("dd/MM/yyyy") : "Nunca", Subtitle = HistorialPurgas.Count > 0 ? HistorialPurgas[0].Estado : "Sin registros" },
        new KpiCardModel { Title = "TOTAL PURGAS", Value = HistorialPurgas.Count.ToString(), Subtitle = "Ejecutadas históricamente" }
    };

    public IEnumerable<KpiCardModel> MonthlySummaryKPIs => new[]
    {
        new KpiCardModel { Title = "INGRESOS TOTALES", Value = $"S/ {TotalIngresosMes:N2}", Subtitle = "Período seleccionado", Color = "#059669" },
        new KpiCardModel { Title = "EGRESOS TOTALES", Value = $"S/ {TotalEgresosMes:N2}", Subtitle = "Período seleccionado", Color = "#DC2626" },
        new KpiCardModel { Title = "BALANCE NETO", Value = $"S/ {BalanceNetoMes:N2}", Subtitle = "Ingresos - Egresos", Color = BalanceNetoMes >= 0 ? "#059669" : "#DC2626" },
        new KpiCardModel { Title = "MARGEN", Value = TotalIngresosMes > 0 ? $"{(BalanceNetoMes / TotalIngresosMes * 100):N1}%" : "0%", Subtitle = "Rentabilidad", Color = BalanceNetoMes >= 0 ? "#059669" : "#DC2626" }
    };

    public IEnumerable<KpiCardModel> ReturnsSummaryKPIs => new[]
    {
        new KpiCardModel { Title = "TOTAL DEVOLUCIONES", Value = TotalDevoluciones.ToString(), Subtitle = "En el período" },
        new KpiCardModel { Title = "MONTO TOTAL", Value = $"S/ {MontoTotalDevoluciones:N2}", Subtitle = "Valor devuelto" },
        new KpiCardModel { Title = "PROMEDIO POR DEV.", Value = TotalDevoluciones > 0 ? $"S/ {MontoTotalDevoluciones / TotalDevoluciones:N2}" : "S/ 0.00", Subtitle = "Ticket promedio" },
        new KpiCardModel { Title = "MOTIVO FRECUENTE", Value = MotivoFrecuente, Subtitle = "Causa principal" }
    };

    partial void OnSelectedReportTypeChanged(string value)
    {
        _ = LoadReportData();
    }

    partial void OnSelectedDateChanged(DateTime value)
    {
        if (SelectedReportType == "Diario")
            _ = LoadReportData();
    }

    partial void OnStartDateChanged(DateTime value)
    {
        if (SelectedReportType is "Balances Mensuales" or "Balance de Devoluciones")
            _ = LoadReportData();
    }

    partial void OnEndDateChanged(DateTime value)
    {
        if (SelectedReportType is "Balances Mensuales" or "Balance de Devoluciones")
            _ = LoadReportData();
    }

    [RelayCommand]
    private async Task LoadReportData()
    {
        IsLoading = true;
        try
        {
            await Task.Delay(300); // Simulate async loading

            switch (SelectedReportType)
            {
                case "Diario":
                    LoadDailyReport();
                    break;
                case "Purga de Sistema":
                    LoadSystemPurge();
                    break;
                case "Balances Mensuales":
                    LoadMonthlyBalances();
                    break;
                case "Balance de Devoluciones":
                    LoadReturnsBalance();
                    break;
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void LoadDailyReport()
    {
        // Simulated data - replace with actual service calls
        TotalVentasDiarias = 12450.50m;
        TotalOrdenesDiarias = 47;
        TotalMesasAtendidas = 32;
        TicketPromedioDiario = 264.90m;

        DetalleVentasDiarias.Clear();
        DetalleVentasDiarias.Add(new DetalleVentaDiaria { Hora = "12:00", Mesa = "Mesa 5", Items = 3, Total = 185.00m, Mesero = "Juan Pérez" });
        DetalleVentasDiarias.Add(new DetalleVentaDiaria { Hora = "12:30", Mesa = "Mesa 12", Items = 5, Total = 420.50m, Mesero = "María García" });
        DetalleVentasDiarias.Add(new DetalleVentaDiaria { Hora = "13:15", Mesa = "Mesa 3", Items = 2, Total = 95.00m, Mesero = "Carlos López" });
        DetalleVentasDiarias.Add(new DetalleVentaDiaria { Hora = "14:00", Mesa = "Mesa 8", Items = 4, Total = 310.00m, Mesero = "Ana Martínez" });
        DetalleVentasDiarias.Add(new DetalleVentaDiaria { Hora = "19:30", Mesa = "Mesa 1", Items = 6, Total = 580.00m, Mesero = "Pedro Sánchez" });
        DetalleVentasDiarias.Add(new DetalleVentaDiaria { Hora = "20:15", Mesa = "Mesa 7", Items = 3, Total = 245.00m, Mesero = "Laura Torres" });
        DetalleVentasDiarias.Add(new DetalleVentaDiaria { Hora = "21:00", Mesa = "Mesa 10", Items = 4, Total = 375.50m, Mesero = "Juan Pérez" });
    }

    private void LoadSystemPurge()
    {
        RegistrosAntiguos = 15420;
        EspacioLiberadoMB = 245;

        HistorialPurgas.Clear();
        HistorialPurgas.Add(new LogPurga { Fecha = DateTime.Today.AddDays(-7), RegistrosEliminados = 3200, EspacioMB = 45, Duracion = "00:02:15", Estado = "Completado" });
        HistorialPurgas.Add(new LogPurga { Fecha = DateTime.Today.AddDays(-14), RegistrosEliminados = 2800, EspacioMB = 38, Duracion = "00:01:58", Estado = "Completado" });
        HistorialPurgas.Add(new LogPurga { Fecha = DateTime.Today.AddDays(-21), RegistrosEliminados = 4100, EspacioMB = 62, Duracion = "00:03:12", Estado = "Completado" });
        HistorialPurgas.Add(new LogPurga { Fecha = DateTime.Today.AddDays(-30), RegistrosEliminados = 5320, EspacioMB = 85, Duracion = "00:04:05", Estado = "Completado" });
    }

    private void LoadMonthlyBalances()
    {
        BalancesMensuales.Clear();
        BalancesMensuales.Add(new BalanceMes { Mes = "Enero 2025", Ingresos = 185000, Egresos = 95000, Balance = 90000 });
        BalancesMensuales.Add(new BalanceMes { Mes = "Febrero 2025", Ingresos = 165000, Egresos = 88000, Balance = 77000 });
        BalancesMensuales.Add(new BalanceMes { Mes = "Marzo 2025", Ingresos = 210000, Egresos = 105000, Balance = 105000 });
        BalancesMensuales.Add(new BalanceMes { Mes = "Abril 2025", Ingresos = 195000, Egresos = 92000, Balance = 103000 });
        BalancesMensuales.Add(new BalanceMes { Mes = "Mayo 2025", Ingresos = 225000, Egresos = 110000, Balance = 115000 });
        BalancesMensuales.Add(new BalanceMes { Mes = "Junio 2025", Ingresos = 200000, Egresos = 98000, Balance = 102000 });

        TotalIngresosMes = 1180000;
        TotalEgresosMes = 588000;
        BalanceNetoMes = 592000;
    }

    private void LoadReturnsBalance()
    {
        Devoluciones.Clear();
        Devoluciones.Add(new DevolucionItem { Fecha = DateTime.Today.AddDays(-2), Orden = "ORD-0045", Producto = "Lomo Saltado", Cantidad = 1, Monto = 45.00m, Motivo = "Plato frío", Mesero = "Juan Pérez" });
        Devoluciones.Add(new DevolucionItem { Fecha = DateTime.Today.AddDays(-3), Orden = "ORD-0052", Producto = "Ceviche Mixto", Cantidad = 1, Monto = 52.00m, Motivo = "Sabor extraño", Mesero = "María García" });
        Devoluciones.Add(new DevolucionItem { Fecha = DateTime.Today.AddDays(-5), Orden = "ORD-0061", Producto = "Papa a la Huancaína", Cantidad = 2, Monto = 36.00m, Motivo = "Porción pequeña", Mesero = "Carlos López" });
        Devoluciones.Add(new DevolucionItem { Fecha = DateTime.Today.AddDays(-7), Orden = "ORD-0073", Producto = "Arroz con Pollo", Cantidad = 1, Monto = 38.00m, Motivo = "Plato frío", Mesero = "Ana Martínez" });
        Devoluciones.Add(new DevolucionItem { Fecha = DateTime.Today.AddDays(-10), Orden = "ORD-0089", Producto = "Chaufa de Mariscos", Cantidad = 1, Monto = 48.00m, Motivo = "Error en pedido", Mesero = "Pedro Sánchez" });
        Devoluciones.Add(new DevolucionItem { Fecha = DateTime.Today.AddDays(-12), Orden = "ORD-0095", Producto = "Tiradito", Cantidad = 1, Monto = 42.00m, Motivo = "Sabor extraño", Mesero = "Laura Torres" });

        TotalDevoluciones = 7;
        MontoTotalDevoluciones = 261.00m;
        MotivoFrecuente = "Plato frío (2)";
    }

    [RelayCommand]
    private async Task EjecutarPurga()
    {
        IsLoading = true;
        try
        {
            await Task.Delay(2000); // Simulate purge operation

            // Add new purge log
            HistorialPurgas.Insert(0, new LogPurga
            {
                Fecha = DateTime.Today,
                RegistrosEliminados = RegistrosAntiguos,
                EspacioMB = EspacioLiberadoMB,
                Duracion = "00:01:45",
                Estado = "Completado"
            });

            RegistrosAntiguos = 0;
            EspacioLiberadoMB = 0;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ExportarReporte()
    {
        // TODO: Implement export functionality (PDF/Excel)
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task ImprimirReporte()
    {
        // TODO: Implement print functionality
        await Task.CompletedTask;
    }
}

// Supporting Models
public class DetalleVentaDiaria
{
    public string Hora { get; set; } = "";
    public string Mesa { get; set; } = "";
    public int Items { get; set; }
    public decimal Total { get; set; }
    public string Mesero { get; set; } = "";
}

public class LogPurga
{
    public DateTime Fecha { get; set; }
    public int RegistrosEliminados { get; set; }
    public long EspacioMB { get; set; }
    public string Duracion { get; set; } = "";
    public string Estado { get; set; } = "";

    public string EspacioMBFormatted => $"{EspacioMB} MB";
    public string RegistrosEliminadosFormatted => RegistrosEliminados.ToString("N0");
}

public class BalanceMes
{
    public string Mes { get; set; } = "";
    public decimal Ingresos { get; set; }
    public decimal Egresos { get; set; }
    public decimal Balance { get; set; }
}

public class DevolucionItem
{
    public DateTime Fecha { get; set; }
    public string Orden { get; set; } = "";
    public string Producto { get; set; } = "";
    public int Cantidad { get; set; }
    public decimal Monto { get; set; }
    public string Motivo { get; set; } = "";
    public string Mesero { get; set; } = "";
}

public class KpiCardModel
{
    public string Title { get; set; } = "";
    public string Value { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public string Color { get; set; } = "#FFFFFF";
}