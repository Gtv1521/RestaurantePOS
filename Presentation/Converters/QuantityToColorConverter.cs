using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace MiComanderaApp.Presentation.Converters
{
    public class QuantityToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not double availableQuantity || parameter is not double minimumQuantity)
            {
                return Brushes.Transparent;
            }

            object brush;
            if (availableQuantity <= 0)
            {
                Application.Current!.TryFindResource("ErrorBrush", out brush!);
                return brush ?? Brushes.Red;
            }
            if (availableQuantity <= minimumQuantity)
            {
                Application.Current!.TryFindResource("WarningBrush", out brush!);
                return brush ?? Brushes.Yellow;
            }
            
            Application.Current!.TryFindResource("SuccessBrush", out brush!);
            return brush ?? Brushes.Green;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
