using Backend.Core;
using Shared;

namespace Backend.Services;

public interface ICaptchaService
{
    Task<Result<byte[], Error>> GenerateCaptchaImage(string code);
    string GenerateCaptchaToken(string code);
    bool ValidateAnswer(string token, string answer, out string? verificationToken);
    bool ValidateVerificationToken(string token);
    string GenerateCaptchaCode();
}