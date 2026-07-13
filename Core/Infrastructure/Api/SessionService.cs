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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace MiComanderaApp.Infrastructure.Api
{
    public class SessionService : ISession<SessionModel>
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;

        public SessionService(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _url = $"{apiSettings.Value.BaseUrl}/api/Auth";
        }

        public SessionModel? Model { get; set; }

        public async Task<SessionModel> LoginAsync(string pinCode)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_url}/Login", new { pin = pinCode });

            if (response.StatusCode == HttpStatusCode.BadRequest)
                throw new BadRequestException("Solicitud incorrecta");

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new NotFoundException("Usuario no encontrado");

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<SessionModel?>();
            Model = result;
            return result ?? throw new InvalidOperationException("La respuesta del servidor fue nula.");
        }

        public Task<bool> LogoutAsync()
        {
            throw new NotImplementedException();
        }
    }
}