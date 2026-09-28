using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using System.Text.Json;
using System.Diagnostics;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Exceptions;
using MiComanderaApp.Models;
using Microsoft.Extensions.Options;
using RestaurantePOS.Core.Domain.Models;

namespace MiComanderaApp.Core.Infrastructure.Api
{
    public class RecetaRepository : IRecetaRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public RecetaRepository(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClientFactory.CreateClient("MiComanderaApi");
            _baseUrl = $"{apiSettings.Value.BaseUrl}/api/Recipe";
        }

        public async Task<string?> CreateAsync(RecetaRequest data)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{data.ProductId}", data.RecipeItems);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => new BadRequestException(error),
                    HttpStatusCode.NotFound => new NotFoundException(error),
                    _ => new HttpRequestException($"Error {(int)response.StatusCode}: {error}")
                };
            }
            var content = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"RecetaRepository.CreateAsync response content: {content}");

            if (string.IsNullOrWhiteSpace(content))
                return null;

            // Try parse as JSON
            try
            {
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.String)
                {
                    return root.GetString();
                }

                if (root.ValueKind == JsonValueKind.Number)
                {
                    return root.GetRawText();
                }

                if (root.ValueKind == JsonValueKind.Object)
                {
                    // Common fields that might contain the id/result
                    if (root.TryGetProperty("id", out var idProp))
                    {
                        if (idProp.ValueKind == JsonValueKind.String) return idProp.GetString();
                        if (idProp.ValueKind == JsonValueKind.Number) return idProp.GetRawText();
                    }
                    if (root.TryGetProperty("result", out var resProp))
                    {
                        if (resProp.ValueKind == JsonValueKind.String) return resProp.GetString();
                        if (resProp.ValueKind == JsonValueKind.Number) return resProp.GetRawText();
                    }

                    // Fallback: return the full object as string
                    return content;
                }
            }
            catch (JsonException)
            {
                // Not JSON, return raw content
            }

            // If it's not JSON, return raw trimmed content
            return content.Trim();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => new BadRequestException(error),
                    HttpStatusCode.NotFound => new NotFoundException(error),
                    _ => new HttpRequestException($"Error {(int)response.StatusCode}: {error}")
                };
            }
            // Read response as string and try to interpret as boolean in a robust way.
            var content = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"RecetaRepository.UpdateAsync response content: {content}");

            if (string.IsNullOrWhiteSpace(content))
            {
                return true;
            }

            // Try direct boolean parse (handles "true" or true)
            if (bool.TryParse(content.Trim().Trim('"'), out var boolResult))
            {
                return boolResult;
            }

            try
            {
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                // If the root is a boolean JSON token
                if (root.ValueKind == JsonValueKind.True || root.ValueKind == JsonValueKind.False)
                {
                    return root.GetBoolean();
                }

                // If the root is an object, try to find the first boolean property
                if (root.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in root.EnumerateObject())
                    {
                        if (prop.Value.ValueKind == JsonValueKind.True || prop.Value.ValueKind == JsonValueKind.False)
                        {
                            return prop.Value.GetBoolean();
                        }
                    }
                }
            }
            catch (JsonException je)
            {
                Debug.WriteLine($"Failed to parse JSON response: {je.Message}");
            }

            // As a fallback, assume success when we can't determine a boolean
            return true;
        }

        public async Task<List<RecetaModel>> GetAllRecetasAsync(int pageNumber, int pageSize)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/all?pageNumber={pageNumber}&pageSize={pageSize}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => new BadRequestException(error),
                    HttpStatusCode.NotFound => new NotFoundException(error),
                    _ => new HttpRequestException($"Error {(int)response.StatusCode}: {error}")
                };
            }
            var result = await response.Content.ReadFromJsonAsync<List<RecetaModel>>();
            return result ?? throw new InvalidOperationException("La respuesta del servidor fue nula.");
        }

        public Task<RecetaModel> GetAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<RecetaModel>> GetByProductoIdAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(int productId, List<RecetaRequest> receta)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(string id, RecetaRequest data)
        {
            // The API expects a JSON array of recipe item objects in the request body.
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{id}", data.RecipeItems);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => new BadRequestException(error),
                    HttpStatusCode.NotFound => new NotFoundException(error),
                    _ => new HttpRequestException($"Error {(int)response.StatusCode}: {error}")
                };
            }
            // Read response as string and try to interpret as boolean in a robust way.
            var content = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"RecetaRepository.UpdateAsync response content: {content}");

            if (string.IsNullOrWhiteSpace(content))
            {
                return true;
            }

            if (bool.TryParse(content.Trim().Trim('"'), out var boolResult))
            {
                return boolResult;
            }

            try
            {
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.True || root.ValueKind == JsonValueKind.False)
                {
                    return root.GetBoolean();
                }

                if (root.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in root.EnumerateObject())
                    {
                        if (prop.Value.ValueKind == JsonValueKind.True || prop.Value.ValueKind == JsonValueKind.False)
                        {
                            return prop.Value.GetBoolean();
                        }
                    }
                }
            }
            catch (JsonException je)
            {
                Debug.WriteLine($"Failed to parse JSON response in UpdateAsync: {je.Message}");
            }

            return true;
        }
    }
}
