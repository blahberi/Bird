namespace Frontend.Services;

public interface IAuthService
{
    event Func<Task> OnStateChanged;
    string UserId { get; }
    DateTime Expiration { get; }
    bool IsAuthorized { get; }
    Task<bool> IsAuthorizedAsync { get; }
    Task<string> UsernameAsync { get; }
    Task SetToken(string token);
    Task LogOutAsync();
}