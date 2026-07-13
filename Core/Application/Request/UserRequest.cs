using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiComanderaApp.Request
{
    public class UserRequest
    {
        public string NombreCompleto { get; set; } = "";
        public string Email { get; set; } = "";
        public int Pin { get; set; } = 0;
        public int Rol { get; set; } = 0;
        public bool Activo { get; set; } = false;
    }
}