using Backend.Core;
using Shared;
using Shared.DTOs;

namespace Backend.Services
{
    public interface IUserService
    {
        Task<Result> RegisterUser(UserRegistartion registration);
        Task<Result<string>> LoginUser(UserLogin request);
        Task<Result<User>> GetUserByUsername(string username);
        Task<Result<User>> GetUserById(int id);
    }
}
