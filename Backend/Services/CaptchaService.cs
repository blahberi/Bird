using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Backend.Core;
using Backend.Extensions;
using Microsoft.IdentityModel.Tokens;
using Shared;
using Shared.Extensions;

namespace Backend.Services;

internal class CaptchaService : ICaptchaService
{
    private readonly HttpClient captchaGeneratorClient = new HttpClient();
    private readonly byte[] aesKey = new byte[32];
    private readonly byte[] aesIv = new byte[16];
    private readonly byte[] jwtSecret = new byte[32];
    private readonly JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
    private readonly TokenValidationParameters validationParameters;
    private readonly SigningCredentials signingCredentials;

    public CaptchaService()
    {
        this.captchaGeneratorClient.BaseAddress = new Uri("http://localhost:8000");
        this.GenerateKeys();
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
    public async Task<byte[]> GenerateCaptchaImage(string code)
    {
        HttpResponseMessage response = await this.captchaGeneratorClient.GetAsync($"GenerateCaptcha/{code}");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to generate captcha");
        }
        return await response.Content.ReadAsByteArrayAsync();
    }

    public string GenerateCaptchaToken(string code)
    {
        string challengeProof = this.GenerateChallengeProof(code);
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim("challengeProof", challengeProof)
            }),
            Expires = DateTime.UtcNow.AddMinutes(2),
            SigningCredentials = this.signingCredentials
        };
        SecurityToken token = this.tokenHandler.CreateToken(tokenDescriptor);
        return this.tokenHandler.WriteToken(token);
    }

    public Result<string, Error> ValidateAnswer(string token, string answer)
    {
        try
        {
            ClaimsPrincipal principal = this.tokenHandler.ValidateToken(token, this.validationParameters, out SecurityToken _);
            string? challengeProof = principal.FindFirst("challengeProof")?.Value;
            if (this.GenerateChallengeProof(answer) != challengeProof)
            {
                return Error.CreateErr<string>("Invalid captcha");
            }

            return this.GenerateVerificationToken().ToOkResult();
        }
        catch
        {
            return Error.CreateErr<string>("Invalid captcha");
        }
    }

    public bool ValidateVerificationToken(string token)
    {
        try
        {
            this.tokenHandler.ValidateToken(token, this.validationParameters, out SecurityToken _);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GenerateCaptchaCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        Random random = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private string GenerateChallengeProof(string code)
    {
        byte[] codeBytes = Encoding.UTF8.GetBytes(code);
        byte[] cypherCode = this.EncryptCode(codeBytes);
        byte[] challengeProof = this.HashCypherCode(cypherCode);
        return Convert.ToBase64String(challengeProof);
    }

    private byte[] EncryptCode(byte[] codeBytes)
    {
        using Aes aes = Aes.Create();
        aes.Key = this.aesKey;
        aes.IV = this.aesIv;
        using MemoryStream memoryStream = new MemoryStream();
        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            cryptoStream.Write(codeBytes, 0, codeBytes.Length);
        }
        return memoryStream.ToArray();
    }

    private byte[] HashCypherCode(byte[] cypherCode)
    {
        return SHA256.HashData(cypherCode);
    }

    private string GenerateVerificationToken()
    {
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddMinutes(2),
            SigningCredentials = this.signingCredentials
        };
        SecurityToken token = this.tokenHandler.CreateToken(tokenDescriptor);
        return this.tokenHandler.WriteToken(token);
    }

    private void GenerateKeys()
    {
        using RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        rng.GetBytes(this.aesKey);
        rng.GetBytes(this.aesIv);
        rng.GetBytes(this.jwtSecret);
    }
}