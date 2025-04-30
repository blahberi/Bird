using Backend.Core;
using Shared;

namespace Backend.Services;

public interface ICaptchaService
{
    Task<byte[]> GenerateCaptchaImage(string code);
    string GenerateCaptchaToken(string code);
    Result<string, Error> ValidateAnswer(string token, string answer);
    bool ValidateVerificationToken(string token);
    string GenerateCaptchaCode();
}