using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace MiComanderaApp.Presentation.Converters;

public class DecimalToBrushConverter : IValueConverter
{
    public static readonly DecimalToBrushConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is decimal decimalValue)
        {
            return decimalValue >= 0 
                ? new SolidColorBrush(Color.Parse("#059669")) // Green for positive
                : new SolidColorBrush(Color.Parse("#DC2626")); // Red for negative
        }
        if (value is double doubleValue)
        {
            return doubleValue >= 0 
                ? new SolidColorBrush(Color.Parse("#059669")) 
                : new SolidColorBrush(Color.Parse("#DC2626"));
        }
        if (value is int intValue)
        {
            return intValue >= 0 
                ? new SolidColorBrush(Color.Parse("#059669")) 
                : new SolidColorBrush(Color.Parse("#DC2626"));
        }
        return new SolidColorBrush(Color.Parse("#6B7280"));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}