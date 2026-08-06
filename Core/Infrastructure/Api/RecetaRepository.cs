using System;
using System.Linq;
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

        public RecetaRepository(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClientFactory.CreateClient("MiComanderaApi");
            _baseUrl = $"{apiSettings.Value.BaseUrl}/api/Recipe";
        }

        public Task<string?> CreateAsync(RecetaRequest data)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<RecetaModel>> GetAllRecetasAsync(int pageNumber, int pageSize)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/all?pageNumber={pageNumber}&pageSize={pageSize}");
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
            var result = await response.Content.ReadFromJsonAsync<List<RecetaModel>>();
            return result ?? throw new InvalidOperationException("La respuesta del servidor fue nula.");
        }

        public Task<RecetaModel> GetAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<RecetaModel>> GetByProductoIdAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(int productId, List<RecetaRequest> receta)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(string id, RecetaRequest data)
        {
            throw new NotImplementedException();
        }
    }
}
