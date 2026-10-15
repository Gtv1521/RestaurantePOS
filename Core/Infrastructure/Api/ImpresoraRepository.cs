using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Exceptions;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using Microsoft.Extensions.Options;

namespace MiComanderaApp.Core.Infrastructure.Api
{
    public class ImpresoraRepository
    {
        private readonly HttpClient _http;
        private readonly string _url;

        public ImpresoraRepository(IHttpClientFactory factory, IOptions<ApiSettings> settings)
        {
            _http = factory.CreateClient("MiComanderaApi");
            _url = $"{settings.Value.BaseUrl}/api/Printer";
        }

        public async Task<List<ImpresoraModel>> GetImpresorasAsync()
        {
            var response = await _http.GetAsync(_url);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => new BadRequestException(error),
                    HttpStatusCode.Unauthorized => new UnauthorizedAccessException(error),
                    _ => new HttpRequestException($"Error {(int)response.StatusCode}: {error}")
                };
            }

            var result = await response.Content.ReadFromJsonAsync<List<ImpresoraModel>>();
            return result ?? new List<ImpresoraModel>();
        }

        public async Task<bool> ImprimirAsync(int impresoraId, string texto)
        {
            var response = await _http.PostAsJsonAsync($"{_url}/raw", new
            {
                printerId = impresoraId,
                texto
            });

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => new BadRequestException(error),
                    HttpStatusCode.NotFound => new NotFoundException(error),
                    HttpStatusCode.Unauthorized => new UnauthorizedAccessException(error),
                    _ => new HttpRequestException($"Error {(int)response.StatusCode}: {error}")
                };
            }

            return true;
        }
    }
}
