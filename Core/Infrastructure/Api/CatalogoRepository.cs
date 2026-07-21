using System;
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
    public class CatalogoRepository : IMultipleCrud<CatalogoModel, CatalogoRequest>
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;

        public CatalogoRepository(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            _url = $"{apiSettings.Value.BaseUrl}/api/Category";
            _httpClient = httpClient;
        }

        public async Task<string?> CreateAsync(CatalogoRequest data)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_url}", data);

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

            var result = await response.Content.ReadFromJsonAsync<string>();

            return result ?? throw new InvalidOperationException("La respuesta del servidor fue nula.");
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{_url}/{id}");

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

            return result ?? throw new InvalidOperationException("La respuesta del servidor fue nula.");
        }

        public async Task<IEnumerable<CatalogoModel>> GetAllAsync()
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

            return await response.Content.ReadFromJsonAsync<IEnumerable<CatalogoModel>>()
                   ?? Enumerable.Empty<CatalogoModel>();
        }

        public async Task<CatalogoModel> GetAsync(string id)
        {
            var response = await _httpClient.GetAsync($"{_url}/{id}");

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

            return await response.Content.ReadFromJsonAsync<CatalogoModel>()
                   ?? throw new InvalidOperationException("La respuesta del servidor fue nula.");
        }

        public async Task<bool> UpdateAsync(string id, CatalogoRequest data)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_url}", data);

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

            return result ?? throw new InvalidOperationException("La respuesta del servidor fue nula.");

        }
    }
}