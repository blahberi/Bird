using Backend.Core;
using Shared;
using Shared.DTOs;

namespace Backend.Services
{
    public interface IUserService
    {
        Task<Result<None, Error>> RegisterUser(UserRegistartion registration);
        Task<Result<string, Error>> LoginUser(UserLogin request);
        Task<Result<User, Error>> GetUserByUsername(string username);
        Task<Result<User, Error>> GetUserById(int id);
    }
}
