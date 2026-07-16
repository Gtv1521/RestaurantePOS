using System;
using Avalonia.Controls;
using Avalonia.Input;
using MiComanderaApp.ViewModels;

namespace MiComanderaApp.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();

            this.AttachedToVisualTree += (_, _) =>
            {
                Focus();
            };

            this.KeyDown += OnTextInput;
        }

        private void OnTextInput(object? sender, KeyEventArgs e)
        {
            if (DataContext is not LoginViewModel vm)
                return;

            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                var numero = ((int)e.Key - (int)Key.D0).ToString();
                vm.AddDigitCommand.Execute(numero);
            }
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                var numero = ((int)e.Key - (int)Key.NumPad0).ToString();
                vm.AddDigitCommand.Execute(numero);
            }
            else if (e.Key == Key.Enter)
            {
                vm.LoginCommand.Execute(null);
            }
            else if (e.Key == Key.Back)
            {
                vm.DeleteDigitCommand.Execute(null);
            }
        }
    }
}