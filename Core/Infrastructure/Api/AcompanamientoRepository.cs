using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Exceptions;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using Microsoft.Extensions.Options;

namespace MiComanderaApp.Core.Infrastructure.Api
{
    public class AcompanamientoRepository
    {
        private readonly HttpClient _http;
        private readonly string _url;

        public AcompanamientoRepository(IHttpClientFactory factory, IOptions<ApiSettings> settings)
        {
            _http = factory.CreateClient("MiComanderaApi");
            _url = $"{settings.Value.BaseUrl}/api/Accompaniment";
        }

        public async Task<IEnumerable<AcompanamientoModel>> GetAllAsync()
        {
            var response = await _http.GetAsync(_url);
            EnsureSuccess(response);
            return await response.Content.ReadFromJsonAsync<IEnumerable<AcompanamientoModel>>()
                ?? Enumerable.Empty<AcompanamientoModel>();
        }

        public async Task<AcompanamientoModel?> CreateAsync(AcompanamientoRequest data)
        {
            var response = await _http.PostAsJsonAsync(_url, data);
            EnsureSuccess(response);
            return await response.Content.ReadFromJsonAsync<AcompanamientoModel>();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"{_url}/{id}");
            EnsureSuccess(response);
        }

        private static void EnsureSuccess(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return;
            var error = response.Content.ReadAsStringAsync().Result;
            throw response.StatusCode switch
            {
                HttpStatusCode.BadRequest => new BadRequestException(error),
                HttpStatusCode.NotFound => new NotFoundException(error),
                _ => new HttpRequestException($"Error {(int)response.StatusCode}: {error}")
            };
        }
    }
}
