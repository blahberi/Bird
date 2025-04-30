using Shared;
using Frontend.Core;

namespace Frontend.Services
{
    public interface IApiService
    {
        Task<Result<T, Error>> GetAsync<T>(string path);
        Task<Result<T, Error>> PostAsync<T>(string path, object? data);
        Task<Result<None, Error>> PostAsync(string path, object? data);
        Task<Result<T, Error>> PutAsync<T>(string path, object? data);
        Task<Result<None, Error>> PutAsync(string path, object? data);
        Task<Result<T, Error>> DeleteAsync<T>(string path);
        Task<Result<None, Error>> DeleteAsync(string path);
    }
}
