using Backend.DataAccess;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.DTOs;
using System.Security.Cryptography;
using System.Text;
using Backend.Core;

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

    public async Task<Result> RegisterUser(UserRegistartion registration)
    {
        try
        {
            if (await this.dbContext.Users.AnyAsync(u => u.Username == registration.Username))
            {
                return Result.FailureResult("Username already exists");
            }

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
            return Result.SuccessResult();
        }
        catch (Exception e)
        {
            // Log the exception
            Console.WriteLine($"Error registering user: {e}");
            return Result.FailureResult("An error occurred while registering the user");
        }
    }

    public async Task<Result<string>> LoginUser(UserLogin login)
    {
        try
        {
            User? user = await this.dbContext.Users.FirstOrDefaultAsync(u => u.Username == login.Username);
            if (user == null)
            {
                return Result<string>.FailureResult("Invalid username or password");
            }

            string hash = ComputeHash(login.Password, user.PasswordSalt);
            if (hash != user.PasswordHash)
            {
                return Result<string>.FailureResult("Invalid username or password");
            }

            string token = this.authService.GenerateToken(user.Id);

            return Result<string>.SuccessResult(token);
        }
        catch (Exception e)
        {
            // Log the exception
            Console.WriteLine($"Error logging in user: {e}");
            return Result<string>.FailureResult("An error occurred during login");
        }
    }

    public async Task<Result<User>> GetUserByUsername(string username)
    {
        try
        {
            User? user = await this.dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return Result<User>.FailureResult("User not found");
            }

            return Result<User>.SuccessResult(user);
        }
        catch (Exception e)
        {
            // Log the exception
            Console.WriteLine($"Error getting user by username: {e}");
            return Result<User>.FailureResult("An error occurred while retrieving the user");
        }
    }

    public async Task<Result<User>> GetUserById(int id)
    {
        try
        {
            User? user = await this.dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return Result<User>.FailureResult("User not found");
            }
            return Result<User>.SuccessResult(user);
        }
        catch (Exception e)
        {
            // Log the exception
            Console.WriteLine($"Error getting user by id: {e}");
            return Result<User>.FailureResult("An error occurred while retrieving the user");
        }
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
