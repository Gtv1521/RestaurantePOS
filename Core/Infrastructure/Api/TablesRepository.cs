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
    public class TablesRepository : ISingleCrud<TableModel, TableRequest>
    {
<<<<<<< HEAD
        public Task<string?> CreateAsync(TableRequest data)
=======
        private readonly HttpClient _httpClient;
        private readonly string _url;

        public TablesRepository(IHttpClientFactory _factory, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = _factory.CreateClient("MiComanderaApi");
            _url = $"{apiSettings.Value.BaseUrl}/api/Tables";
        }

        public Task<string> CreateAsync(TableRequest data)
>>>>>>> origin/Gustavo
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TableModel>> GetAllAsync(string id, int? page, int? size)
        {
            var response = await _httpClient.GetAsync($"{_url}/GetAllTables");
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

        public Task<bool> UpdateAsync(string id, TableRequest data)
        {
            throw new NotImplementedException();
        }
    }
}