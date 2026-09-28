using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace MiComanderaApp.Core.Domain.Interfaces
{
    public interface IFocusService
    {
        TextBox? CurrentFocusedTextBox { get; }
        event Action<TextBox?>? FocusChanged;
        void RegisterTextBox(TextBox textBox);
        void UnregisterTextBox(TextBox textBox);
        void SetFocus(TextBox textBox);
        void ClearFocus();
        List<TextBox> GetRegisteredTextBoxes();
    }
}
