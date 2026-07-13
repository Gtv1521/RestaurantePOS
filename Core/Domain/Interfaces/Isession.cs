using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiComanderaApp.Interfaces
{
    public interface ISession<R>
    {
        R? Model { get; set; }
        Task<R> LoginAsync(string pinCode);
        Task<bool> LogoutAsync();
    }
}