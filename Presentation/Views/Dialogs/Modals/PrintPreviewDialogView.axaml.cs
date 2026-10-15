using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MiComanderaApp.Views.Dialogs.Modals
{
    public partial class PrintPreviewDialogView : Window
    {
        public PrintPreviewDialogView()
        {
            InitializeComponent();
        }

        private async void OnCopiar(object? sender, RoutedEventArgs e)
        {
            if (this.FindControl<TextBox>("TxtDocumento") is { } textBox
                && !string.IsNullOrEmpty(textBox.Text)
                && TopLevel.GetTopLevel(this)?.Clipboard is { } clipboard)
            {
                await clipboard.SetTextAsync(textBox.Text);
            }
        }
    }
}
