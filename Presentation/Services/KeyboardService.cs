using System;
using MiComanderaApp.Core.Domain.Interfaces;

namespace MiComanderaApp.Presentation.Services
{
    public class KeyboardService : IKeyboardService
    {
        public event Action<string>? KeyPressed;

        public void SendKey(string key)
        {
            KeyPressed?.Invoke(key);
        }

        public void ShowKeyboard()
        {
            KeyPressed?.Invoke("SHOW_KEYBOARD");
        }

        public void HideKeyboard()
        {
            KeyPressed?.Invoke("HIDE_KEYBOARD");
        }
    }
}
