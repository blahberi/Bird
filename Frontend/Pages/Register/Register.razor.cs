using Frontend.Components.CaptchaComponent;
using Frontend.Models;
using Frontend.Services;
using Microsoft.AspNetCore.Components;
using Shared;

namespace Frontend.Pages.Register;

public class RegisterBase : ComponentBase
{
    protected RegisterModel registerModel = new();
    protected CaptchaComponent captchaComponent = default!;

    [Inject]
    protected IUserService UserService { get; set; } = default!;

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    protected async Task HandleRegister()
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
