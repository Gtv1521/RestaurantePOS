using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MiComanderaApp.Presentation.Views.Animations;

public partial class LoadingAnimatedView : UserControl
{
    public static readonly StyledProperty<string> StatusTextProperty =
        AvaloniaProperty.Register<LoadingAnimatedView, string>(
            nameof(StatusText), "Cargando...");

    public string StatusText
    {
        get => GetValue(StatusTextProperty);
        set => SetValue(StatusTextProperty, value);
    }

    public LoadingAnimatedView()
    {
        InitializeComponent();
    }
}
