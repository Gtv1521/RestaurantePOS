using System.Collections.Generic;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net;
using MiComanderaApp.Exceptions;
using System;
using System.Linq;

namespace MiComanderaApp.Core.Infrastructure.Api
{
    public class IngredienteRepository : IMultipleCrud<IngredienteModel, IngredienteRequest>
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;
        public IngredienteRepository(IHttpClientFactory httpClient, IOptions<ApiSettings> apiSettings){
            _url = $"{apiSettings.Value.BaseUrl}/api/Ingredient";
            _httpClient = httpClient.CreateClient("MiComanderaApi");
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
            var result = await response.Content.ReadFromJsonAsync<IngredienteModel>();
            return result?.Id.ToString() ?? throw new InvalidOperationException("No se pudo crear el producto.");
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
            return result ?? Enumerable.Empty<IngredienteModel>();
        }
        


        public Task<IngredienteModel> GetAsync(string id)
        {
            throw new System.NotImplementedException();
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
            return response.IsSuccessStatusCode;
        }
    }
}