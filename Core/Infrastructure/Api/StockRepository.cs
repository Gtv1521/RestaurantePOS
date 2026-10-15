using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MiComanderaApp.Exceptions;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using Microsoft.Extensions.Options;

namespace MiComanderaApp.Core.Infrastructure.Api
{
    public class StockRepository
    {
        private readonly HttpClient _http;
        private readonly string _url;

        public StockRepository(IHttpClientFactory factory, IOptions<ApiSettings> settings)
        {
            _http = factory.CreateClient("MiComanderaApi");
            _url = $"{settings.Value.BaseUrl}/api/Ingredient";
        }

        public async Task<bool> AjustarAsync(int ingredientId, decimal newQuantity, string reason)
        {
            var response = await _http.PostAsJsonAsync($"{_url}/{ingredientId}/adjust", new
            {
                newQuantity,
                reason
            });

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

            return true;
        }
    }
}
