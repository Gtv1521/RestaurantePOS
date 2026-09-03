using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MiComanderaApp.Core.Domain.Interfaces;

namespace MiComanderaApp.Presentation.Services
{
    public class FocusService : IFocusService
    {
        private TextBox? _currentFocusedTextBox;
        private readonly List<TextBox> _registeredTextBoxes = new();
        public TextBox? CurrentFocusedTextBox => _currentFocusedTextBox;
        public event Action<TextBox?>? FocusChanged;
        public void RegisterTextBox(TextBox textBox)
        {
            if (_registeredTextBoxes.Contains(textBox)) return;
            _registeredTextBoxes.Add(textBox);
            textBox.GotFocus += OnTextBoxGotFocus;
            textBox.LostFocus += OnTextBoxLostFocus;
        }
        public void UnregisterTextBox(TextBox textBox)
        {
            if (!_registeredTextBoxes.Remove(textBox)) return;
            textBox.GotFocus -= OnTextBoxGotFocus;
            textBox.LostFocus -= OnTextBoxLostFocus;

            if (_currentFocusedTextBox == textBox)
            {
                _currentFocusedTextBox = null;
                FocusChanged?.Invoke(null);
            }
        }
        public void SetFocus(TextBox textBox)
        {
            if (textBox == null || !textBox.IsEnabled) return;
            textBox.Focus();
        }
        public void ClearFocus()
        {
            _currentFocusedTextBox = null;
            FocusChanged?.Invoke(null);
        }
        private void OnTextBoxGotFocus(object? sender, GotFocusEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                _currentFocusedTextBox = textBox;
                FocusChanged?.Invoke(textBox);
                System.Diagnostics.Debug.WriteLine($"Foco en: {textBox.Name ?? "Sin nombre"}");
            }
        }
        private void OnTextBoxLostFocus(object? sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                System.Diagnostics.Debug.WriteLine($"Perdió foco: {textBox.Name ?? "Sin nombre"}");
            }
        }
        public List<TextBox> GetRegisteredTextBoxes()
        {
            return _registeredTextBoxes.ToList();
        }
    }
}
