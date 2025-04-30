using Backend.Core;
using Shared;
using Shared.DTOs;

namespace Backend.Services
{
    public interface IUserService
    {
        Task RegisterUser(UserRegistartion registration);
        Task<string> LoginUser(UserLogin request);
        Task<User> GetUserByUsername(string username);
        Task<User> GetUserById(int id);
    }
}
