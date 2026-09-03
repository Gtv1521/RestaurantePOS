using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace MiComanderaApp.Presentation.Converters
{
    public class MayusculasConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // value = IsMayusculas (true/false)
            // parameter = "a" (la letra)

            if (parameter is string letra && !string.IsNullOrEmpty(letra))
            {
                if (value is bool esMayuscula && esMayuscula)
                {
                    return letra.ToUpper();
                }
                return letra.ToLower();
            }
            return parameter;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LedConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isActive)
            {
                return isActive ? new SolidColorBrush(Color.Parse("#edf1f7")) : new SolidColorBrush(Color.Parse("#a09898"));
            }
            return new SolidColorBrush(Colors.White);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}