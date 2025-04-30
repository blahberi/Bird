using Shared;
using Shared.DTOs;
using Frontend.Core;
namespace Frontend.Services;

public interface IUserService
{
    Task<string> Username { get; }
    Task<Result<UserLoginResponse, Error>> LoginAsync(string username, string password);
    Task<Result<None, Error>> RegisterAsync(string username, string password, string email, string firstName, string lastName, string country, string city, string gender);
    Task<Result<UserInfoResponse, Error>> GetUserInfoAsync();
}