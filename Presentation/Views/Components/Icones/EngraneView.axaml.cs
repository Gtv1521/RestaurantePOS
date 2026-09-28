using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace MiComanderaApp.Presentation.Views.Components.Icon;

public partial class EngraneView : UserControl
{
    public static readonly StyledProperty<IBrush?> ColorProperty =
        AvaloniaProperty.Register<EngraneView, IBrush?>(
            nameof(Color));

    public IBrush? Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    } 


    public static readonly StyledProperty<double> IconWidthProperty =
        AvaloniaProperty.Register<EngraneView, double>(
            nameof(IconWidth),
            50);

    public double IconWidth
    {
        get => GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    public static readonly StyledProperty<double> IconHeightProperty =
        AvaloniaProperty.Register<EngraneView, double>(
            nameof(IconHeight),
            50);

    public double IconHeight
    {
        get => GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
    }
 
    public EngraneView()
    {
        InitializeComponent();
    }
}