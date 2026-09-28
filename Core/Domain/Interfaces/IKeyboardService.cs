using System;

namespace MiComanderaApp.Core.Domain.Interfaces
{
    public interface IKeyboardService
    {
        event Action<string>? KeyPressed;
        void SendKey(string key);
        void ShowKeyboard();
        void HideKeyboard();
    }
}
