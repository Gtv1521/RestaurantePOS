using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiComanderaApp.Models
{
    public class SessionModel
    {
        public int UserId { get; set; } = 0;
        public string Token { get; set; } = string.Empty;
        public string TokenRefresh { get; set; } = string.Empty;
        public int RolId { get; set; }
    }
}