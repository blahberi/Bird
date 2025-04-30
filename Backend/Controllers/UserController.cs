using System.Runtime.CompilerServices;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Shared;
using Shared.DTOs;
using Backend.Core;

namespace Backend.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : AuthControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {
            int? userId = this.AuthorizeUser();
            if (userId == null)
            {
                return this.UnauthorizedUser();
            }

            Result<User> userResult = await this.userService.GetUserById(userId.Value);
            if (!userResult.Success || userResult.Value == null)
            {
                return this.NotFound(new ErrorResponse { Error = userResult.Error });
            }

            User user = userResult.Value;
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
            if (!this.ModelState.IsValid)
            {
                Dictionary<string, string> errors = this.ModelState
                    .Where(x => x.Value.Errors.Any())
                    .ToDictionary(
                        x => x.Key,
                        x => String.Join(", ", x.Value.Errors.Select(e => e.ErrorMessage))
                    );
                return this.BadRequest(new ErrorResponse { Details = errors });
            }

            if (!this.VerifyUser())
            {
                return this.UnverifiedUser();
            }

            Result result = await this.userService.RegisterUser(request);
            if (!result.Success)
            {
                return this.Conflict(new ErrorResponse { Error = result.Error });
            }
            return this.Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin request)
        {
            Result<string> result = await this.userService.LoginUser(request);

            if (!result.Success || result.Value == null)
            {
                return this.Unauthorized(new ErrorResponse { Error = result.Error });
            }

            string token = result.Value;
            return this.Ok(new UserLoginResponse { Token = token });
        }

        private bool VerifyUser()
        {
            bool? isHuman = this.HttpContext.Items["IsHuman"] as bool?;
            return isHuman ?? false;
        }

        private UnauthorizedObjectResult UnverifiedUser()
        {
            return this.Unauthorized(new ErrorResponse { Error = "Not verified as human" });
        }
    }
}