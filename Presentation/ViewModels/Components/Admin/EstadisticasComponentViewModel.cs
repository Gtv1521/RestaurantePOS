using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MiComanderaApp.Models;
using SkiaSharp;

namespace MiComanderaApp.ViewModels.Components.Admin;
public partial class EstadisticasComponentViewModel : ViewModelBase
{
    [ObservableProperty]
    public ISeries[]? _monthlySeries;
    [ObservableProperty]
    public ISeries[]? _trimestreSeries;
    [ObservableProperty]
    public ISeries[]? _anualSeries;

    public List<SalesCardModel>? SalesCards { get; set; }
    public List<WaiterPerformanceModel>? WaitersPerformance { get; set; }

    public EstadisticasComponentViewModel()
    {
        LoadEstadisticas();
        LoadData();
        LoadMeseros();
    }

    public void LoadData()
    {
        SalesCards = new List<SalesCardModel>
        {
            new() {
                Title = "VENTAS MENSUALES",
                Amount = "S/ 61,000",
                PercentageText = "+15.1% vs período anterior",
                ChartSeries = MonthlySeries! // Tu configuración existente de LiveCharts
            },
            new() {
                Title = "VENTAS TRIMESTRALES",
                Amount = "S/ 172,000",
                PercentageText = "+13.9% vs período anterior",
                ChartSeries = TrimestreSeries!
            },
            new() {
                Title = "VENTAS ANUALES",
                Amount = "S/ 610,000",
                PercentageText = "+5.2% vs período anterior",
                ChartSeries = AnualSeries!
            }
        };
    }


    public void LoadEstadisticas()
    {
        MonthlySeries = new ISeries[]
        {
            new LineSeries<double>
            {

                Values = new double[] { 60, 60.5, 61, 61, 62, 61, 61.5 }, // Datos de ejemplo
                GeometrySize = 0, // Oculta los puntos redondos para que quede plano como tu imagen
                Stroke = new SolidColorPaint(SKColor.Parse("#5cb334")) { StrokeThickness = 2 }, // Línea naranja
                Fill = new LinearGradientPaint(
                    new SKColor[] { SKColor.Parse("#465e6dc8"), SKColors.Transparent },
                    new SKPoint(0.5f, 0), // Punto inicial (arriba)
                    new SKPoint(0.5f, 1))

            }

        };

        TrimestreSeries = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = new double[] { 58000, 59500, 61000, 60500, 61200, 61000, 52500 },
                GeometrySize = 0, // Oculta los puntos redondos para que quede plano como tu imagen
                Stroke = new SolidColorPaint(SKColor.Parse("#2831e0")) { StrokeThickness = 2 }, // Línea naranja
                Fill = new LinearGradientPaint(
                    new SKColor[] { SKColor.Parse("#465e6dc8"), SKColors.Transparent },
                    new SKPoint(0.5f, 0), // Punto inicial (arriba)
                    new SKPoint(0.5f, 1)) // Punto final (abajo)
            }
        };

        AnualSeries = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = new double[] { 58000, 61000, 62500, 63200, 64000, 52500 },
                GeometrySize = 0, // Oculta los puntos redondos para que quede plano como tu imagen
                Stroke = new SolidColorPaint(SKColor.Parse("#28e08a")) { StrokeThickness = 2 }, // Línea naranja
                Fill = new LinearGradientPaint(
                    new SKColor[] { SKColor.Parse("#465e6dc8"), SKColors.Transparent },
                    new SKPoint(0.5f, 0), // Punto inicial (arriba)
                    new SKPoint(0.5f, 1)) // Punto final (abajo)
            }
        };
    }

    public void LoadMeseros()
    {
        WaitersPerformance = new List<WaiterPerformanceModel>
        {
            new() { Name = "Carlos M.", Percentage = 90 },
            new() { Name = "Ana Q.", Percentage = 75 },
            new() { Name = "Luis T.", Percentage = 62 },
            new() { Name = "Sofía R.", Percentage = 50 },
            new() { Name = "Pedro C.", Percentage = 40 }
        };
    }
}
