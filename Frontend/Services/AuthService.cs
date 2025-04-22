using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace Frontend.Services;

public class AuthService : IAuthService
{
    private readonly IStorageService storageService;
    private readonly HttpClient client;
    private readonly JwtSecurityTokenHandler tokenHandler;
    private readonly Task retrieveTokenTask;

    public AuthService(IStorageService storageService, HttpClient client)
    {
        this.storageService = storageService;
        this.client = client;
        this.tokenHandler = new JwtSecurityTokenHandler();
        this.Expiration = DateTime.MinValue;

        this.retrieveTokenTask = this.RetrieveToken();
    }

    public bool IsAuthorized => this.Expiration > DateTime.UtcNow;
    public Task<bool> IsAuthorizedAsync => this.GetIsAuthorizedAsync();
    public string UserId { get; private set; }
    public Task<string> UsernameAsync => this.GetUsernameAsync();
    public DateTime Expiration { get; private set; }

    public event Func<Task> OnStateChanged;

    public async Task SetToken(string token)
    {
        JwtSecurityToken jwtToken = this.tokenHandler.ReadJwtToken(token);
        this.UserId = jwtToken.Subject;
        this.Expiration = jwtToken.ValidTo;

        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await this.storageService.SetItem("token", token);
        this.StateChangedInvoke();
    }

    private async Task<bool> GetIsAuthorizedAsync()
    {
        await this.retrieveTokenTask;
        return this.IsAuthorized;
    }

    private async Task<string> GetUsernameAsync()
    {
        await this.retrieveTokenTask;
        return this.UserId;
    }

    private async Task RetrieveToken()
    {
        string token = await this.storageService.GetItem("token");
        if (!string.IsNullOrEmpty(token))
        {
            await this.SetToken(token);
        }
    }

    public async Task LogOutAsync()
    {
        this.UserId = string.Empty;
        this.Expiration = DateTime.MinValue;
        this.client.DefaultRequestHeaders.Authorization = null;
        await this.storageService.RemoveItem("token");
        this.StateChangedInvoke();
    }

    private void StateChangedInvoke()
    {
        this.OnStateChanged.Invoke();
    }
}