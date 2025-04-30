using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Shared;
using Shared.DTOs;
using Shared.Extensions;
using Frontend.Core;

namespace Frontend.Services
{
    internal class ApiService : IApiService
    {
        private readonly HttpClient client;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public ApiService(HttpClient client)
        {
            this.client = client;
            this.jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<Result<T, Error>> GetAsync<T>(string path)
        {
            try
            {
                HttpResponseMessage response = await this.client.GetAsync(path);
                return await this.HandleResponse<T>(response);
            }
            catch
            {
                return Error.CreateErrResult<T>("Failed to connect to the server");
            }
        }

        public async Task<Result<T, Error>> PostAsync<T>(string path, object? data)
        {
            try
            {
                string json = JsonSerializer.Serialize(data);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await this.client.PostAsync(path, content);
                return await this.HandleResponse<T>(response);
            }
            catch
            {
                return Error.CreateErrResult<T>("Failed to connect to the server");
            }
        }

        public async Task<Result<None, Error>> PostAsync(string path, object? data)
        {
            try
            {
                string json = JsonSerializer.Serialize(data);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await this.client.PostAsync(path, content);
                return await this.HandleResponse(response);
            }
            catch
            {
                return Error.CreateErrResult<None>("Failed to connect to the server");
            }
        }

        public async Task<Result<T, Error>> PutAsync<T>(string path, object? data)
        {
            try
            {
                string json = JsonSerializer.Serialize(data);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await this.client.PutAsync(path, content);
                return await this.HandleResponse<T>(response);
            }
            catch
            {
                return Error.CreateErrResult<T>("Failed to connect to the server");
            }
        }

        public async Task<Result<None, Error>> PutAsync(string path, object? data)
        {
            try
            {
                string json = JsonSerializer.Serialize(data);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await this.client.PutAsync(path, content);
                return await this.HandleResponse(response);
            }
            catch
            {
                return Error.CreateErrResult<None>("Failed to connect to the server");
            }
        }

        public async Task<Result<T, Error>> DeleteAsync<T>(string path)
        {
            try
            {
                HttpResponseMessage response = await this.client.DeleteAsync(path);
                return await this.HandleResponse<T>(response);
            }
            catch
            {
                return Error.CreateErrResult<T>("Failed to connect to the server");
            }
        }

        public async Task<Result<None, Error>> DeleteAsync(string path)
        {
            try
            {
                HttpResponseMessage response = await this.client.DeleteAsync(path);
                return await this.HandleResponse(response);
            }
            catch
            {
                return Error.CreateErrResult<None>("Failed to connect to the server");
            }
        }

        private async Task<Result<T, Error>> HandleResponse<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                return await HandleErrorResponse<T>(response);
            }
            string json = await response.Content.ReadAsStringAsync();
            try
            {
                T? data = JsonSerializer.Deserialize<T>(json, this.jsonSerializerOptions);
                if (data == null)
                {
                    Console.WriteLine($"Failed to deserialize response: {json}");
                    return Error.CreateErrResult<T>("Failed to process server response");
                }
                return Error.CreateOkResult(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deserialization error: {ex}, JSON: {json}");
                return Error.CreateErrResult<T>("Failed to process server response");
            }
        }

        private async Task<Result<None, Error>> HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                return await HandleErrorResponse<None>(response);
            }
            return Error.CreateOkResult(None.Value);
        }

        private async Task<Result<T, Error>> HandleErrorResponse<T>(HttpResponseMessage response)
        {
            string json = await response.Content.ReadAsStringAsync();
            try
            {
                ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(json, this.jsonSerializerOptions);
                if (errorResponse == null)
                {
                    Console.WriteLine($"Unknown error: {response.StatusCode}, Content: {json}");
                    return Error.CreateErrResult<T>("An error occurred while processing your request");
                }
                if (string.IsNullOrWhiteSpace(errorResponse.Error) && errorResponse.Details.Count == 0)
                {
                    Console.WriteLine($"Empty error response: {response.StatusCode}, Content: {json}");
                    return Error.CreateErrResult<T>("An error occurred while processing your request");
                }

                return Error.CreateErrResult<T>(errorResponse.Error, errorResponse.Details);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing error response: {ex}, StatusCode: {response.StatusCode}, Content: {json}");
                return Error.CreateErrResult<T>("An unexpected error occurred");
            }
        }
    }
}
