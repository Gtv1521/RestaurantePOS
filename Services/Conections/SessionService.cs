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

namespace MiComanderaApp.Services.Conections
{
    public class SessionService : ISession<SessionModel>
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        public SessionService(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
        }

        public async Task<SessionModel?> LoginAsync(string pinCode)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiSettings.BaseUrl}/api/Session/Login", pinCode);

            if (response.StatusCode == HttpStatusCode.BadRequest)
                throw new BadRequestException("Solicitud incorrecta");

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new NotFoundException("Usuario no encontrado");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<SessionModel>();
        }

        public Task<bool> LogoutAsync()
        {
            throw new NotImplementedException();
        }
    }
}