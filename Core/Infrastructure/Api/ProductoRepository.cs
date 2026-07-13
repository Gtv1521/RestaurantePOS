using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ProductoRepository : IMultipleCrud<ProductoModel, ProductoRequest>, IGetList<ProductoModel>
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;
        public ProductoRepository(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _url = $"{apiSettings.Value.BaseUrl}/api/Product";
        }
        public Task<string?> CreateAsync(ProductoRequest data)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductoModel>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProductoModel>> GetAllDataAsync(string id)
        {
            var response = await _httpClient.GetAsync($"{_url}/category/{id}");

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

            return await response.Content.ReadFromJsonAsync<IEnumerable<ProductoModel>>()
                   ?? Enumerable.Empty<ProductoModel>();
        }

        public Task<ProductoModel> GetAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(string id, ProductoRequest data)
        {
            throw new NotImplementedException();
        }
    }
}