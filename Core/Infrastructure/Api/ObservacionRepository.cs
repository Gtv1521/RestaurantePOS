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
    public class ObservacionRepository : IMultipleCrud<ObservacionModel, ObservacionRequest>
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;

        public ObservacionRepository(
            IHttpClientFactory factory,
            IOptions<ApiSettings> settings
        )
        {
            _httpClient = factory.CreateClient("MiComanderaApi");
            _url = $"{settings.Value.BaseUrl}/api/Observacion";
        }
        public async Task<string> CreateAsync(ObservacionRequest data)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_url}", data);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                System.Console.WriteLine(error);
                throw response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => new BadRequestException(error),
                    HttpStatusCode.NotFound => new NotFoundException(error),
                    HttpStatusCode.Unauthorized => new UnauthorizedAccessException(error),
                    _ => new HttpRequestException(
                        $"Error {(int)response.StatusCode}: {error}")
                };
            }
            var result = await response.Content.ReadFromJsonAsync<ObservacionModel>();
            return result?.Id.ToString() ?? throw new InvalidOperationException("No se pudo crear la observación.");
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

        public async Task<IEnumerable<ObservacionModel>> GetAllAsync()
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
            return await response.Content.ReadFromJsonAsync<IEnumerable<ObservacionModel>>()
               ?? Enumerable.Empty<ObservacionModel>();
        }

        public async Task<ObservacionModel> GetAsync(string id)
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

            var result = await response.Content.ReadFromJsonAsync<ObservacionModel>();

            return result ?? throw new InvalidOperationException("La respuesta del servidor fue nula.");

        }

        public Task<bool> UpdateAsync(string id, ObservacionRequest data)
        {
            throw new NotImplementedException();
        }
    }
}