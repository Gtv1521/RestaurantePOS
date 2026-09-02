using Avalonia.Controls;

namespace MiComanderaApp.Core.Domain.Interfaces
{
    public interface IFocusService
    {
        void SetFocus(TextBox textBox);
        TextBox? CurrentTextBox { get; }
    }
}
