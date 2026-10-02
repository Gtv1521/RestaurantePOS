using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Exceptions;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using Microsoft.Extensions.Options;

namespace MiComanderaApp.Core.Infrastructure.Api
{
    public class ComandaRepository
    {
        private readonly HttpClient _http;
        private readonly string _url;

        public ComandaRepository(IHttpClientFactory factory, IOptions<ApiSettings> settings)
        {
            _http = factory.CreateClient("MiComanderaApi");
            _url = $"{settings.Value.BaseUrl}/api/Venta/comanda";
        }

        public async Task<bool> CrearAsync(ComandaCreateRequest data)
        {
            var response = await _http.PostAsJsonAsync(_url, data);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => new BadRequestException(error),
                    HttpStatusCode.NotFound => new NotFoundException(error),
                    _ => new HttpRequestException($"Error {(int)response.StatusCode}: {error}")
                };
            }
            return true;
        }
    }
}
