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

        }

        protected override void OnDetaching()
        {
            if (_focusService != null)
            {
                _focusService.UnregisterTextBox(AssociatedObject!);
            }

            base.OnDetaching();
        }
    }
}