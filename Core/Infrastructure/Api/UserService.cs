using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MiComanderaApp.Exceptions;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.Request;
using Microsoft.Extensions.Options;

namespace MiComanderaApp.Infrastructure.Api
{
    public class UserService : IMultipleCrud<UsuarioModel, UserRequest>
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;
        public UserService(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            _url = $"{apiSettings.Value.BaseUrl}/api/Users";
            _httpClient = httpClient;
        }

        public async Task<string> CreateAsync(UserRequest data)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_url}/CreateUser", data);

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
            var response = await _httpClient.DeleteAsync($"{_url}/DeleteUser/{id}");

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

        public async Task<IEnumerable<UsuarioModel>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync($"{_url}/GetAllUsers");

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

            return await response.Content.ReadFromJsonAsync<IEnumerable<UsuarioModel>>()
                   ?? Enumerable.Empty<UsuarioModel>();
        }

        public async Task<UsuarioModel> GetAsync(string id)
        {
            var response = await _httpClient.GetAsync($"{_url}/GetUserById/{id}");

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

            return await response.Content.ReadFromJsonAsync<UsuarioModel>()
                   ?? throw new InvalidOperationException("La respuesta del servidor fue nula.");
        }

        public async Task<bool> UpdateAsync(string id, UserRequest data)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_url}/UpdateUser/{id}", data);

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