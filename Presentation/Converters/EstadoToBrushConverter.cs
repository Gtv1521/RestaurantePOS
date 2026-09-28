using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace MiComanderaApp.Presentation.Converters;

public class EstadoToBrushConverter : IValueConverter
{
    public static readonly EstadoToBrushConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string estado)
        {
            return estado switch
            {
                "Completado" => new SolidColorBrush(Color.Parse("#059669")), // Green
                "En Progreso" => new SolidColorBrush(Color.Parse("#EA580C")), // Orange
                "Fallido" => new SolidColorBrush(Color.Parse("#DC2626")), // Red
                "Pendiente" => new SolidColorBrush(Color.Parse("#6366F1")), // Blue
                _ => new SolidColorBrush(Color.Parse("#6B7280")) // Gray
            };
        }
        return new SolidColorBrush(Color.Parse("#6B7280"));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}