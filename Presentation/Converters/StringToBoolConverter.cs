using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace MiComanderaApp.Presentation.Converters;

public class StringToBoolConverter : IValueConverter
{
    public static readonly StringToBoolConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str && parameter is string param)
        {
            var options = param.Split('|');
            return Array.Exists(options, opt => str.Equals(opt, StringComparison.OrdinalIgnoreCase));
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}