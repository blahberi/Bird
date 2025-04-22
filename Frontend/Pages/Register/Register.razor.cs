using Frontend.Components.CaptchaComponent;
using Frontend.Models;
using Frontend.Services;
using Microsoft.AspNetCore.Components;
using Shared;

namespace Frontend.Pages.Register;

public partial class Register : ComponentBase
{
    private RegisterModel registerModel = new();
    private CaptchaComponent captchaComponent;

    [Inject]
    private IUserService UserService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    private async Task HandleRegister()
    {
        if (!captchaComponent.IsHuman)
        {
            return;
        }

        Result result = await UserService.RegisterAsync(
            registerModel.Username,
            registerModel.Password,
            registerModel.Email,
            registerModel.FirstName,
            registerModel.LastName,
            registerModel.Country,
            registerModel.City,
            registerModel.Gender);

        if (!result.Success)
        {
            return;
        }

        NavigationManager.NavigateTo("/login");
    }
}