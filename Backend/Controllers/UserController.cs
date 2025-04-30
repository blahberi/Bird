using Backend.Services;
using Backend.Extensions;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.DTOs;
using Backend.Core;
using Shared.Extensions;

namespace Backend.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {
            int userId = this.AuthorizeUser();
            User user = await this.userService.GetUserById(userId);
            UserInfoResponse response = new UserInfoResponse
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Country = user.Country,
                City = user.City,
                Gender = user.Gender
            };

            return this.Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistartion request)
        {
            if (!this.IsHuman())
            {
                return this.UnverifiedUser();
            }

            return await this.userService.RegisterUser(request)
                .ToActionResultAsync();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin request)
        {
            return await this.userService.LoginUser(request)
                .MapAsync(async token => new UserLoginResponse {Token = token})
                .ToActionResultAsync();
        }

        private UnauthorizedObjectResult UnverifiedUser()
        {
            return this.Unauthorized(new ErrorResponse { Error = "Not verified as human" });
        }
    }
}