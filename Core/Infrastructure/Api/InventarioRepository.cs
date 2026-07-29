using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Exceptions;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using Microsoft.Extensions.Options;
using RestaurantePOS.Core.Application.Request;

namespace MiComanderaApp.Core.Infrastructure.Api
{
    public class InventarioRepository : IMultipleCrud<IngredienteModel, IngredienteRequest>
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;

        public InventarioRepository(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _url = $"{apiSettings.Value.BaseUrl}/api/Ingredient";
        }

        public async Task<string?> CreateAsync(IngredienteRequest data)
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

            return result ?? throw new InvalidOperationException("No se pudo crear el ingrediente.");
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
            return result ?? throw new InvalidOperationException("No se pudo Borrar el ingrediente.");
        }

        public async Task<IEnumerable<IngredienteModel>> GetAllAsync()
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
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<IngredienteModel>>();
            return result ?? throw new InvalidOperationException("No se pudo obtener los ingredientes.");
        }

        public async Task<IngredienteModel> GetAsync(string id)
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
            var result = await response.Content.ReadFromJsonAsync<IngredienteModel>();
            return result ?? throw new InvalidOperationException("No se pudo obtener el ingrediente.");
        }
        public async Task<bool> UpdateAsync(string id, IngredienteRequest data)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_url}/{id}", data);
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
            return result ?? throw new InvalidOperationException("No se pudo actualizar el ingrediente.");  
        }
    }
}
