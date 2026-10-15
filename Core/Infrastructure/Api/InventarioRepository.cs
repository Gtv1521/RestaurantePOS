using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using Microsoft.Extensions.Options;

namespace MiComanderaApp.Core.Infrastructure.Api
{
    public class InventarioRepository
    {
        private readonly HttpClient _http;
        private readonly string _url;

        public InventarioRepository(IHttpClientFactory factory, IOptions<ApiSettings> settings)
        {
            _http = factory.CreateClient("MiComanderaApi");
            _url = $"{settings.Value.BaseUrl}/api/Admin/inventario";
        }

        public async Task<InventarioReporteModel?> GetReporteAsync()
        {
            var response = await _http.GetAsync(_url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<InventarioReporteModel>();
        }
    }
}
