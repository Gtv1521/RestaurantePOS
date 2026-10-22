using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiComanderaApp.Interfaces
{
    public interface ISession<R>
    {
        R? Model { get; }
        Task<R> LoginAsync(string pinCode);
        Task<bool> LogoutAsync();
        Task ClearSessionAsync();

        /// <summary>
        /// Verifica un PIN sin modificar la sesión actual.
        /// Devuelve el modelo del usuario dueño del PIN, o null si es inválido.
        /// </summary>
        Task<R?> VerifyPinAsync(string pinCode);
    }
}