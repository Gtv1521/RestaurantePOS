using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MiComanderaApp.Models
{
    public class SessionModel
    {
        // La API devuelve el id del usuario en la propiedad "id"; la mapeamos a UserId.
        [JsonPropertyName("id")]
        public int UserId { get; set; } = 0;
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string TokenRefresh { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Iniciales => string.Join("", NombreCompleto.Split(' ').Select(x => x[0])).ToUpper();
        public DateTime Expira { get; set; }
    }
}