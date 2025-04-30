using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Shared;
using Shared.DTOs;
using Shared.Extensions;

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

        public async Task<Result<T>> GetAsync<T>(string path)
        {
            try
            {
                HttpResponseMessage response = await this.client.GetAsync(path);
                return await this.HandleResponse<T>(response);
            }
            catch
            {
                return Result<T>.FailureResult("Failed to connect to the server");
            }
        }

        public async Task<Result<T>> PostAsync<T>(string path, object? data)
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
                return Result<T>.FailureResult("Failed to connect to the server");
            }
        }

        public async Task<Result> PostAsync(string path, object? data)
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
                return Result.FailureResult("Failed to connect to the server");
            }
        }

        public async Task<Result<T>> PutAsync<T>(string path, object? data)
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
                return Result<T>.FailureResult("Failed to connect to the server");
            }
        }

        public async Task<Result> PutAsync(string path, object? data)
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
                return Result.FailureResult("Failed to connect to the server");
            }
        }

        public async Task<Result<T>> DeleteAsync<T>(string path)
        {
            try
            {
                HttpResponseMessage response = await this.client.DeleteAsync(path);
                return await this.HandleResponse<T>(response);
            }
            catch
            {
                return Result<T>.FailureResult("Failed to connect to the server");
            }
        }

        public async Task<Result> DeleteAsync(string path)
        {
            try
            {
                HttpResponseMessage response = await this.client.DeleteAsync(path);
                return await this.HandleResponse(response);
            }
            catch
            {
                return Result.FailureResult("Failed to connect to the server");
            }
        }

        private async Task<Result<T>> HandleResponse<T>(HttpResponseMessage response)
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
                    return Result<T>.FailureResult("Failed to process server response");
                }
                return Result<T>.SuccessResult(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deserialization error: {ex}, JSON: {json}");
                return Result<T>.FailureResult("Failed to process server response");
            }
        }

        private async Task<Result<None, string>> HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                return await HandleErrorResponse<None>(response);
            }
            return Result<None, string>.CreateOk(None.Value);
        }

        private async Task<Result<T, string>> HandleErrorResponse<T>(HttpResponseMessage response)
        {
            string json = await response.Content.ReadAsStringAsync();
            try
            {
                ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(json, this.jsonSerializerOptions);
                if (errorResponse == null)
                {
                    Console.WriteLine($"Unknown error: {response.StatusCode}, Content: {json}");
                    return Result<T, string>.CreateErr("An error occurred while processing your request");
                }
                if (errorResponse.Details != null && errorResponse.Details.Count > 0)
                {
                    return Result<T, string>.CreateErr(errorResponse.Details);
                }
                if (errorResponse.Error != null)
                {
                    return Result<T, string>.CreateErr(errorResponse.Error);
                }
                Console.WriteLine($"Empty error response: {response.StatusCode}, Content: {json}");
                return Result<T, string>.CreateErr("An error occurred while processing your request");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing error response: {ex}, StatusCode: {response.StatusCode}, Content: {json}");
                return Result<T, string>.CreateErr("An unexpected error occurred");
            }
        }
    }
}
