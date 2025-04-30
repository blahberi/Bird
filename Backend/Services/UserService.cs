using Backend.DataAccess;
using Backend.Extensions;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.DTOs;
using System.Security.Cryptography;
using System.Text;
using Backend.Core;
using Shared.Extensions;

namespace Backend.Services;

internal class UserService : IUserService
{
    private readonly IAuthService authService;
    private readonly ApplicationDbContext dbContext;

    public UserService(IAuthService authService, ApplicationDbContext dbContext)
    {
        this.authService = authService;
        this.dbContext = dbContext;
    }

    public async Task<Result<None, Error>> RegisterUser(UserRegistartion registration)
    {
        return await CheckModel(registration).ToAsync()
            .AndThenAsync(async _ =>
            {
                return await this.dbContext.Users
                    .AnyAsync(u => u.Username == registration.Username)
                    .ErrIfAsync(exists => exists, "Username already exists");
            })
            .AndThenAsync(async _ =>
            {
                (string passwordHash, string salt) = HashPassword(registration.Password);
                User user = new User
                {
                    Username = registration.Username,
                    PasswordHash = passwordHash,
                    PasswordSalt = salt,
                    FirstName = registration.FirstName,
                    LastName = registration.LastName,
                    Email = registration.Email,
                    Country = registration.Country,
                    City = registration.City,
                    Gender = registration.Gender
                };

                await this.dbContext.Users.AddAsync(user);
                await this.dbContext.SaveChangesAsync();
                return None.Value.ToOkResult();
            });
    }

    public async Task<Result<string, Error>> LoginUser(UserLogin login)
    {
        return await this.dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == login.Username)
            .ToResultAsync("Invalid username or password")
            .AndThenAsync(async user =>
            {
                string hash = ComputeHash(login.Password, user.PasswordSalt);
                if (hash != user.PasswordHash)
                {
                    return Error.CreateErr<string>("Invalid username or password");
                }

                string token = this.authService.GenerateToken(user.Id);
                return Error.CreateOk(token);
            });
    }

    public async Task<Result<User, Error>> GetUserByUsername(string username)
    {
        return await this.dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == username)
            .ToResultAsync("User not found");
    }

    public async Task<Result<User, Error>> GetUserById(int id)
    {
        return await this.dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id)
            .ToResultAsync("User not found");
    }

    private static (string, string) HashPassword(string password)
    {
        int saltLength = 16;
        byte[] saltBytes = new byte[saltLength];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        string saltText = Convert.ToBase64String(saltBytes);
        string hash = ComputeHash(password, saltText);
        return (hash, saltText);
    }

    private static string ComputeHash(string password, string salt)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] saltedPassword = Encoding.UTF8.GetBytes(salt + password);
            byte[] hashBytes = sha256.ComputeHash(saltedPassword);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
