using Shared;

namespace Frontend.Services
{
    public interface IApiService
    {
        Task<Result<T>> GetAsync<T>(string path);
        Task<Result<T>> PostAsync<T>(string path, object? data);
        Task<Result> PostAsync(string path, object? data);
        Task<Result<T>> PutAsync<T>(string path, object? data);
        Task<Result> PutAsync(string path, object? data);
        Task<Result<T>> DeleteAsync<T>(string path);
        Task<Result> DeleteAsync(string path);
    }
}
