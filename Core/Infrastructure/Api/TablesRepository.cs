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
    public class TablesRepository : IMultipleCrud<TableModel, TableRequest>, IOptionsMesas<VentaModel>
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;

        public TablesRepository(IHttpClientFactory _factory, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = _factory.CreateClient("MiComanderaApi");
            _url = $"{apiSettings.Value.BaseUrl}/api/Table";
        }

        public Task<string> CreateAsync(TableRequest data)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TableModel>> GetAllAsync()
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

            return await response.Content.ReadFromJsonAsync<IEnumerable<TableModel>>()
                   ?? Enumerable.Empty<TableModel>();
        }

        public Task<TableModel> GetAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> LiberarMesa(int id)
        {
            var response = await _httpClient.GetAsync($"{_url}/{id}/liberar");
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

        public async Task<VentaModel> OcuparMesa(int id)
        {
            var response = await _httpClient.GetAsync($"{_url}/{id}/ocupar");
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

            var result = await response.Content.ReadFromJsonAsync<VentaModel>();
            return result ?? throw new InvalidOperationException("La respuesta del servidor fue nula.");
        }

        public Task<bool> Reservar(int id, string? nota = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(string id, TableRequest data)
        {
            throw new NotImplementedException();
        }
    }
}