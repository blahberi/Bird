using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;

internal class AuthService : IAuthService
{
    private readonly byte[] jwtSecret = new byte[32];
    private readonly JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
    private readonly TokenValidationParameters validationParameters;
    private readonly SigningCredentials signingCredentials;
    
    private const string SecretFilePath = "auth_secret.key";
    
    public AuthService()
    {
        this.tokenHandler.InboundClaimTypeMap.Clear();

        if (File.Exists(SecretFilePath))
        {
            this.jwtSecret = File.ReadAllBytes(SecretFilePath);
        }
        else
        {
            this.GenerateSecret();
        }
        this.tokenHandler.InboundClaimTypeMap.Clear();
        
        this.validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(this.jwtSecret),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };
        
        this.signingCredentials = new SigningCredentials(new SymmetricSecurityKey(this.jwtSecret), SecurityAlgorithms.HmacSha256Signature);
    }
    public string GenerateToken(int userId)
    {
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = this.GetIdentity(userId),
            Expires = DateTime.UtcNow.AddDays(30),
            SigningCredentials = this.signingCredentials
        };
        SecurityToken token = this.tokenHandler.CreateToken(tokenDescriptor);
        return this.tokenHandler.WriteToken(token);
    }
    
    public bool ValidateToken(string token, out int? userId)
    {
        try
        {
            ClaimsPrincipal principal = this.tokenHandler.ValidateToken(token, this.validationParameters, out SecurityToken _);
            string? userIdStr = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrWhiteSpace(userIdStr))
            {
                userId = null;
                return false;
            }
            userId = int.Parse(userIdStr);
            return true;
        }
        catch
        {
            userId = null;
            return false;
        }
    }

    public string RefreshToken(string token)
    {
        if (!this.ValidateToken(token, out _))
        {
            return string.Empty;
        }

        JwtSecurityToken jwtToken = this.tokenHandler.ReadJwtToken(token);
        if (jwtToken.ValidTo.Subtract(DateTime.UtcNow) > TimeSpan.FromDays(14))
        {
            return token;
        }

        return this.GenerateToken(int.Parse(jwtToken.Subject));
    }
    
    private ClaimsIdentity GetIdentity(int userId)
    {
        return new ClaimsIdentity(new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())
        });
    }
    
    private void GenerateSecret() {
        using RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        rng.GetBytes(this.jwtSecret);
        File.WriteAllBytes(SecretFilePath, this.jwtSecret);
    }
}