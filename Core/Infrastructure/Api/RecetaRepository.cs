using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Exceptions;
using MiComanderaApp.Models;
using Microsoft.Extensions.Options;
using RestaurantePOS.Core.Domain.Models;

namespace MiComanderaApp.Core.Infrastructure.Api
{
    public class RecetaRepository : IRecetaRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public RecetaRepository(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _baseUrl = apiSettings.Value.BaseUrl;
        }

        public Task<string?> CreateAsync(RecetaRequest data)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<RecetaModel> GetAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<RecetaRequest>> GetByProductoIdAsync(int productId)
        {
            var url = $"{_baseUrl}/api/Product/{productId}/recipe";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw response.StatusCode switch
                {
                    HttpStatusCode.NotFound => new NotFoundException(error),
                    _ => new HttpRequestException($"Error {(int)response.StatusCode}: {error}")
                };
            }
            var result = await response.Content.ReadFromJsonAsync<List<RecetaRequest>>();
            return result ?? new List<RecetaRequest>();
        }

        public async Task UpdateAsync(int productId, List<RecetaRequest> receta)
        {
            // The user's example showed a POST to /product/{productId}
            // A more RESTful approach might be POST or PUT to /product/{productId}/recipe
            var url = $"{_baseUrl}/api/Product/{productId}";
            var response = await _httpClient.PostAsJsonAsync(url, receta);
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
        }

        public Task<bool> UpdateAsync(string id, RecetaRequest data)
        {
            throw new System.NotImplementedException();
        }
    }
}
