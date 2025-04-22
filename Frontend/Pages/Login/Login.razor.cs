using Frontend.Models;
using Frontend.Services;
using Microsoft.AspNetCore.Components;
using Shared;
using Shared.DTOs;

namespace Frontend.Pages.Login;

public partial class Login : ComponentBase
{
    private LoginModel loginModel = new();

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IUserService UserService { get; set; } = default!;

    private async Task HandleLogin()
    {
        Result<UserLoginResponse> result = await UserService.LoginAsync(loginModel.Username, loginModel.Password);
        if (result.Success && result.Value != null)
        {
            NavigationManager.NavigateTo("/");
        }
    }
}