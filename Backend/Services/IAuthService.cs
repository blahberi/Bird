namespace Backend.Services;

public interface IAuthService
{
    string GenerateToken(int userId);
    bool ValidateToken(string token, out int? userId);
    string RefreshToken(string oken);
}