using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using Shared;
using Shared.DTOs.Captcha;

namespace Frontend.Services;

public class CaptchaService : ICaptchaService
{
    private readonly HttpClient client;
    private readonly JwtSecurityTokenHandler tokenHandler;
    private readonly IApiService _apiService;

    private Timer? captchaExpirationTimer;
    private Timer? verificationExpirationTimer;

    public CaptchaService(HttpClient client, IApiService apiService)
    {
        this.client = client;
        this._apiService = apiService;
        this.tokenHandler = new JwtSecurityTokenHandler();

        this.CaptchaExpiration = DateTime.MinValue;
        this.VerificationExpiration = DateTime.MinValue;

        this.OnCaptchaExpiration += this.DisposeCaptchaToken;
        this.OnVerificationExpiration += this.DisposeVerificationToken;
    }

    public DateTime CaptchaExpiration { get; private set; }
    public DateTime VerificationExpiration { get; private set; }
    public bool IsHuman => this.VerificationExpiration > DateTime.UtcNow;
    public bool IsCaptchaValid => this.CaptchaExpiration > DateTime.UtcNow;

    public event EventHandler OnCaptchaExpiration;
    public event EventHandler OnVerificationExpiration;

    public void SetCaptchaToken(string token)
    {
        JwtSecurityToken? jwtToken = this.tokenHandler.ReadJwtToken(token);
        this.CaptchaExpiration = jwtToken.ValidTo;
        this.SetCaptchaExpirationHandler();
    }

    public void SetVerificationToken(string token)
    {
        JwtSecurityToken? jwtToken = this.tokenHandler.ReadJwtToken(token);
        this.VerificationExpiration = jwtToken.ValidTo;
        this.client.DefaultRequestHeaders.Add("Verification", token);
        this.SetVerificationExpirationHandler();
    }

    public async Task<Result<CaptchaDto>> GetCaptchaAsync()
    {
        return await _apiService.GetAsync<CaptchaDto>("Captcha");
    }

    public async Task<Result<VerificationTokenDto>> VerifyCaptchaAsync(string captchaToken, string answer)
    {
        return await _apiService.PostAsync<VerificationTokenDto>("Captcha", new CaptchaAnswer
        {
            Token = captchaToken,
            Answer = answer
        });
    }

    private void SetCaptchaExpirationHandler()
    {
        this.captchaExpirationTimer?.Dispose();
        this.captchaExpirationTimer = this.SetExpirationHandler(this.CaptchaExpiration, this.OnCaptchaExpiration);
    }

    private void SetVerificationExpirationHandler()
    {
        this.verificationExpirationTimer?.Dispose();
        this.verificationExpirationTimer = this.SetExpirationHandler(this.VerificationExpiration, this.OnVerificationExpiration);
    }

    private Timer? SetExpirationHandler(DateTime expiration, EventHandler? handler)
    {
        DateTime now = DateTime.UtcNow;
        if (expiration <= now)
        {
            handler?.Invoke(this, EventArgs.Empty);
            return null;
        }

        TimeSpan timeTilExpiration = expiration - now;
        Timer timer = new Timer(_ => handler?.Invoke(this, EventArgs.Empty), null, timeTilExpiration, Timeout.InfiniteTimeSpan);
        return timer;
    }

    private void DisposeCaptchaToken(object? sender, EventArgs e)
    {
        this.CaptchaExpiration = DateTime.MinValue;
        this.captchaExpirationTimer?.Dispose();
    }

    private void DisposeVerificationToken(object? sender, EventArgs e)
    {
        this.VerificationExpiration = DateTime.MinValue;
        this.client.DefaultRequestHeaders.Remove("Verification");
        this.verificationExpirationTimer?.Dispose();
    }
}