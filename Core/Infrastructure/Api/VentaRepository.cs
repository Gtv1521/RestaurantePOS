using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MiComanderaApp.Exceptions;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using Microsoft.Extensions.Options;

namespace MiComanderaApp.Core.Infrastructure.Api
{
    public class VentaRepository : IOptionVenta
    {
        private readonly HttpClient _http;
        private readonly string _url;

        public VentaRepository(IHttpClientFactory _factory, IOptions<ApiSettings> options)
        {
            _http = _factory.CreateClient("MiComanderaApi");
            _url = $"{options.Value.BaseUrl}/api/Venta";
        }
        public async Task<bool> UptatePax(int id, int pax)
        {
            var response = await _http.PutAsJsonAsync($"{_url}/pax/{id}", pax);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => new BadRequestException(error),
                    HttpStatusCode.NotFound => new NotFoundException(error),
                    _ => new HttpRequestException(
                        $"Error {(int)response.StatusCode}: {error}")
                };
            }
            var result = await response.Content.ReadFromJsonAsync<bool?>();
            return result ?? throw new InvalidOperationException("No se pudo crear el producto.");
        }
    }
}