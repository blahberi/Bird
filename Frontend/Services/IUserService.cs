using Shared;
using Shared.DTOs;

namespace Frontend.Services;

public interface IUserService
{
    Task<string> Username { get; }
    Task<Result<UserLoginResponse>> LoginAsync(string username, string password);
    Task<Result> RegisterAsync(string username, string password, string email, string firstName, string lastName, string country, string city, string gender);
    Task<Result<UserInfoResponse>> GetUserInfoAsync();
}