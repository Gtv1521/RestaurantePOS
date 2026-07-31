using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace MiComanderaApp.Presentation.Converters
{
    public class QuantityToStatusBackgroundBrushConverter : IValueConverter
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
                Application.Current!.TryFindResource("ErrorBrushTransparent", out brush!);
                return brush ?? Brushes.Transparent;
            }
            if (availableQuantity <= minimumQuantity)
            {
                Application.Current!.TryFindResource("WarningBrushTransparent", out brush!);
                return brush ?? Brushes.Transparent;
            }
            
            // For healthy stock, we don't want a background color.
            return Brushes.Transparent;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
