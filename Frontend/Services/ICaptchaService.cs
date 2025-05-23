using Frontend.Core;
using Shared;
using Shared.DTOs.Captcha;

namespace Frontend.Services;

public interface ICaptchaService
{
    DateTime CaptchaExpiration { get; }
    DateTime VerificationExpiration { get; }
    bool IsHuman { get; }
    bool IsCaptchaValid { get; }

    event EventHandler OnCaptchaExpiration;
    event EventHandler OnVerificationExpiration;

    void SetCaptchaToken(string token);
    void SetVerificationToken(string token);

    Task<Result<CaptchaDto, Error>> GetCaptchaAsync();
    Task<Result<VerificationTokenDto, Error>> VerifyCaptchaAsync(string captchaToken, string answer);
}