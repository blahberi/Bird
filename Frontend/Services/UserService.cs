using Shared;
using Shared.DTOs;

namespace Frontend.Services;

public class UserService : IUserService
{
    private readonly IApiService apiService;
    private readonly IAuthService authService;

    public UserService(IApiService apiService, IAuthService authService)
    {
        this.apiService = apiService;
        this.authService = authService;
    }

    public Task<string> Username
    {
        get
        {
            return GetUsernameAsync();
        }
    }

    public async Task<Result<UserLoginResponse>> LoginAsync(string username, string password)
    {
        Result<UserLoginResponse> result = await this.apiService.PostAsync<UserLoginResponse>("users/login", new UserLogin
        {
            Username = username,
            Password = password
        });

        if (result.Success && result.Value != null)
        {
            await authService.SetToken(result.Value.Token);
        }

        return result;
    }

    public async Task<Result> RegisterAsync(string username, string password, string email, string firstName, string lastName, string country, string city, string gender)
    {
        return await this.apiService.PostAsync("users/register", new UserRegistartion
        {
            Username = username,
            Password = password,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Country = country,
            City = city,
            Gender = gender
        });
    }

    public async Task<Result<UserInfoResponse>> GetUserInfoAsync()
    {
        return await this.apiService.GetAsync<UserInfoResponse>("users");
    }

    private async Task<string> GetUsernameAsync()
    {
        try
        {
            var result = await GetUserInfoAsync();
            if (result.Success && result.Value != null)
            {
                return result.Value.Username;
            }
            return string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

}