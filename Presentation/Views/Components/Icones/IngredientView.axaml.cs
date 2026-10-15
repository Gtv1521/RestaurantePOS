using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace MiComanderaApp.Presentation.Views.Components.Icones;

public partial class IngredientView : UserControl
{
    public static readonly StyledProperty<IBrush> ColorProperty =
        AvaloniaProperty.Register<IngredientView, IBrush>(
            nameof(Color), new SolidColorBrush(Colors.Orange));

    public IBrush Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public IngredientView()
    {
        InitializeComponent();
    }
}
