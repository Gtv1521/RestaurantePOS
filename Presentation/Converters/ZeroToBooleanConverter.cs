using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Data.Converters;

namespace MiComanderaApp.Presentation.Converters
{
    public class ZeroToBooleanConverter : IValueConverter
    {
         public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Si el valor es un número y es mayor que 0
            return value is int count && count == 0;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}