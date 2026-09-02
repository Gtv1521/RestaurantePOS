using Avalonia.Controls;
using MiComanderaApp.Core.Domain.Interfaces;

namespace MiComanderaApp.Presentation.Services
{
    public class FocusService : IFocusService
    {
        public TextBox? CurrentTextBox { get; private set; }

        public void SetFocus(TextBox textBox)
        {
            CurrentTextBox = textBox;
        }
    }
}
