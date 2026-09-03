using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using MiComanderaApp.Core.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MiComanderaApp.Presentation.Services
{
    public class KeyboardFocusBehavior : Behavior<TextBox>
    {
        private IFocusService? _focusService;
        protected override void OnAttached()
        {
            base.OnAttached();

            _focusService = Program.AppHost?.Services.GetService<IFocusService>();

            if (_focusService != null)
            {
                _focusService.RegisterTextBox(AssociatedObject!);
            }

            AssociatedObject!.PropertyChanged += TextBox_PropertyChanged;
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.PropertyChanged -= TextBox_PropertyChanged;
            }

            if (_focusService != null)
            {
                _focusService.UnregisterTextBox(AssociatedObject!);
            }

            base.OnDetaching();
        }

        private void TextBox_PropertyChanged(
            object? sender,
            AvaloniaPropertyChangedEventArgs e)
        {
            if (sender is not TextBox textBox)
                return;

            if (e.Property == TextBox.SelectionStartProperty ||
                e.Property == TextBox.SelectionEndProperty ||
                e.Property == TextBox.TextProperty)
            {
                Console.WriteLine(
                    $"PROPERTY -> {e.Property.Name} | " +
                    $"Text='{textBox.Text}' | " +
                    $"Start={textBox.SelectionStart} | " +
                    $"End={textBox.SelectionEnd} | " +
                    $"Focused={textBox.IsFocused}");
            }
        }
    }
}