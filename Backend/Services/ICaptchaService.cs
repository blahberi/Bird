using Shared;

namespace Backend.Services;

public interface ICaptchaService
{
    Task<Result<byte[]>> GenerateCaptchaImage(string code);
    string GenerateCaptchaToken(string code);
    bool ValidateAnswer(string token, string answer, out string? verificationToken);
    bool ValidateVerificationToken(string token);
    string GenerateCaptchaCode();
}