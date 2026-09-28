using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.ViewModels;
using Microsoft.Extensions.Primitives;

namespace MiComanderaApp.Presentation.Views.Components.Generales;

public partial class TecladoComponentViewModel : ViewModelBase
{
    private readonly IFocusService _focusService;
    [ObservableProperty] bool _isMayusculas = false;

    public TecladoComponentViewModel(IFocusService focusService)
    {
        _focusService = focusService;
    }

    [RelayCommand]
    private void InsertKey(string key)
    {
        if (string.IsNullOrEmpty(key))
            return;

        var textBox = _focusService.CurrentFocusedTextBox;

        if (textBox == null)
            return;

        string teclaFinal =
            IsMayusculas ? key.ToUpper() : key.ToLower();

        int cursorPos = textBox.SelectionStart;
        int selectionEnd = textBox.SelectionEnd;

        int selectionLength =
            Math.Abs(selectionEnd - cursorPos);

        textBox.Text ??= string.Empty;

        if (selectionLength > 0)
        {
            int start = Math.Min(cursorPos, selectionEnd);

            textBox.Text =
                textBox.Text.Remove(start, selectionLength);

            textBox.Text =
                textBox.Text.Insert(start, teclaFinal);

            textBox.CaretIndex =
                start + teclaFinal.Length;
        }
        else
        {
            textBox.Text =
                textBox.Text.Insert(cursorPos, teclaFinal);

            textBox.CaretIndex =
                cursorPos + teclaFinal.Length;
        }
    }

    [RelayCommand]
    private void BackSpace()
    {
        var textBox = _focusService.CurrentFocusedTextBox;

        if (textBox == null)
            return;

        if (string.IsNullOrEmpty(textBox.Text))
            return;

        int cursorPos = textBox.SelectionStart;

        if (textBox.SelectionStart != textBox.SelectionEnd)
        {
            int start = Math.Min(
                textBox.SelectionStart,
                textBox.SelectionEnd);

            int length = Math.Abs(
                textBox.SelectionEnd -
                textBox.SelectionStart);

            textBox.Text = textBox.Text.Remove(start, length);

            textBox.CaretIndex = start;
        }
        else if (cursorPos > 0)
        {
            textBox.Text = textBox.Text.Remove(cursorPos - 1, 1);

            textBox.CaretIndex = cursorPos - 1;
        }
    }

    [RelayCommand]
    private void TabKey()
    {
        System.Console.WriteLine("Tabular");
    }

    [RelayCommand]
    private void MoverFocoIzquierda()
    {
        var textBoxActual = _focusService.CurrentFocusedTextBox;
        if (textBoxActual == null) return;

        // Obtener el TextBox anterior en la lista
        var textBoxAnterior = GetPreviousTextBox(textBoxActual);
        if (textBoxAnterior != null)
        {
            textBoxAnterior.Focus();
        }
    }

    [RelayCommand]
    private void MoverFocoDerecha()
    {
        var textBoxActual = _focusService.CurrentFocusedTextBox;
        if (textBoxActual == null) return;

        // Obtener el siguiente TextBox en la lista
        var textBoxSiguiente = GetNextTextBox(textBoxActual);
        if (textBoxSiguiente != null)
        {
            textBoxSiguiente.Focus();
        }
    }

    [RelayCommand]
    private void EnterKey()
    {
        var textBox = _focusService.CurrentFocusedTextBox;

        if (textBox == null)
            return;

        InsertarEnTextBox(textBox, "\n");
    }

    [RelayCommand]
    private void MayusculasKey()
    {
        IsMayusculas = !IsMayusculas;
    }

    [RelayCommand]
    private void SpaceKey()
    {
        var textBox = _focusService.CurrentFocusedTextBox;

        if (textBox == null)
            return;

        InsertarEnTextBox(textBox, " ");
    }

    private void InsertarEnTextBox(TextBox textBox, string key)
    {
        if (string.IsNullOrEmpty(key)) return;

        int cursorPos = textBox.SelectionStart;
        int selectionEnd = textBox.SelectionEnd;
        int selectionLength = Math.Abs(selectionEnd - cursorPos);

        if (textBox.Text == null)
            textBox.Text = string.Empty;

        if (selectionLength > 0)
        {
            int start = Math.Min(cursorPos, selectionEnd);
            textBox.Text = textBox.Text.Remove(start, selectionLength);
            textBox.Text = textBox.Text.Insert(start, key);
            textBox.SelectionStart = start + key.Length;
            textBox.SelectionEnd = start + key.Length;
        }
        else
        {
            textBox.Text = textBox.Text.Insert(cursorPos, key);
            textBox.SelectionStart = cursorPos + key.Length;
            textBox.SelectionEnd = cursorPos + key.Length;
        }

    }

    private TextBox? GetPreviousTextBox(TextBox current)
    {
        // Usar el FocusService para obtener la lista de TextBox registrados
        // Necesitas agregar un método en IFocusService para obtener la lista
        var textBoxes = _focusService.GetRegisteredTextBoxes();

        int index = textBoxes.IndexOf(current);
        if (index > 0)
        {
            return textBoxes[index - 1];
        }
        return textBoxes.LastOrDefault(); // Volver al último si estamos en el primero
    }

    private TextBox? GetNextTextBox(TextBox current)
    {
        var textBoxes = _focusService.GetRegisteredTextBoxes();

        int index = textBoxes.IndexOf(current);
        if (index < textBoxes.Count - 1)
        {
            return textBoxes[index + 1];
        }
        return textBoxes.FirstOrDefault(); // Volver al primero si estamos en el último
    }

}
