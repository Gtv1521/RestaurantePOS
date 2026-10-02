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
        public ProductoRepository(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClientFactory.CreateClient("MiComanderaApi");
            _url = $"{apiSettings.Value.BaseUrl}/api/Product";
        }

        public async Task<int?> CreateAsync(ProductoRequest data)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_url}", data);

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

            if (response.StatusCode == HttpStatusCode.NoContent ||
                response.Content.Headers.ContentLength is null or 0)
            {
                return null; // o "" si prefieres
            }

            var result = await response.Content.ReadFromJsonAsync<ProductoModel?>();
            return result?.Id ?? throw new InvalidOperationException("La respuesta del servidor fue nula.");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_url}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => new BadRequestException(error),
                    HttpStatusCode.NotFound => new NotFoundException(error),
                    HttpStatusCode.Unauthorized => new UnauthorizedAccessException(error),
                    _ => new HttpRequestException(
                        $"Error {(int)response.StatusCode}: {error}")
                };
            }

            if (response.StatusCode == HttpStatusCode.NoContent ||
                response.Content.Headers.ContentLength is null or 0)
            {
                return true; // o "" si prefieres
            }

            var result = await response.Content.ReadFromJsonAsync<bool?>();
            return result ?? throw new InvalidOperationException("La respuesta del servidor fue nula.");
        }

        public async Task<IEnumerable<ProductoModel>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync($"{_url}");

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

        public async Task<IEnumerable<ProductoModel>> GetAllDataAsync(int id)
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

        public async Task<ProductoModel> GetAsync(int id)
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

            var result = await response.Content.ReadFromJsonAsync<ProductoModel>();

            return result ?? throw new InvalidOperationException("La respuesta del servidor fue nula.");
        }

        public async Task<bool> UpdateAsync(int id, ProductoRequest data)
        {
            var payload = new
            {
                id = data.Id,
                name = data.Name,
                description = data.Description,
                price = data.Price,
                categoryId = data.CategoryId,
                isAvailable = data.IsAvailable,
                imageUrl = data.ImageUrl,
                conTermino = data.ConTermino,
                hasAccompaniments = data.HasAccompaniments,
                maxAccompaniments = data.MaxAccompaniments
            };
            var response = await _httpClient.PutAsJsonAsync(_url, payload);
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

            return true;
        }
    }
}