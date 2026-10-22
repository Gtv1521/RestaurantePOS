using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Exceptions;
using MiComanderaApp.Models;
using Microsoft.Extensions.Options;

namespace MiComanderaApp.Core.Infrastructure.Api
{
    public class GeneralSettingsRepository
    {
        private readonly HttpClient _http;
        private readonly string _url;

        public GeneralSettingsRepository(IHttpClientFactory factory, IOptions<ApiSettings> settings)
        {
            _http = factory.CreateClient("MiComanderaApi");
            _url = $"{settings.Value.BaseUrl}/api/GeneralSettings";
        }

        public async Task<GeneralSettingsModel?> GetAsync()
        {
            var response = await _http.GetAsync(_url);
            EnsureSuccess(response);
            return await response.Content.ReadFromJsonAsync<GeneralSettingsModel>();
        }

        public async Task<GeneralSettingsModel?> UpdateAsync(GeneralSettingsRequest request)
        {
            var response = await _http.PutAsJsonAsync(_url, request);
            EnsureSuccess(response);
            return await response.Content.ReadFromJsonAsync<GeneralSettingsModel>();
        }

        private static void EnsureSuccess(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return;
            var error = response.Content.ReadAsStringAsync().Result;
            throw response.StatusCode switch
            {
                HttpStatusCode.BadRequest => new BadRequestException(error),
                HttpStatusCode.NotFound => new NotFoundException(error),
                HttpStatusCode.Unauthorized => new UnauthorizedAccessException(error),
                _ => new HttpRequestException($"Error {(int)response.StatusCode}: {error}")
            };
        }
    }
}
