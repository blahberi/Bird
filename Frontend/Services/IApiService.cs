using Shared;

namespace Frontend.Services
{
    public interface IApiService
    {
        Task<Result<T, string>> GetAsync<T>(string path);
        Task<Result<T, string>> PostAsync<T>(string path, object? data);
        Task<Result<None, string>> PostAsync(string path, object? data);
        Task<Result<T, string>> PutAsync<T>(string path, object? data);
        Task<Result<None, string>> PutAsync(string path, object? data);
        Task<Result<T, string>> DeleteAsync<T>(string path);
        Task<Result<None, string>> DeleteAsync(string path);
    }
}
