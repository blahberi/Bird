using Frontend.Models;
using Frontend.Services;
using Microsoft.AspNetCore.Components;
using Shared;
using Shared.DTOs;

namespace Frontend.Pages.Login;

public class LoginBase : ComponentBase
{
    protected LoginModel loginModel = new();

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    protected IUserService UserService { get; set; } = default!;

    protected async Task HandleLogin()
    {
        Result<UserLoginResponse> result = await UserService.LoginAsync(loginModel.Username, loginModel.Password);
        if (result.Success && result.Value != null)
        {
            NavigationManager.NavigateTo("/");
        }
    }
}
