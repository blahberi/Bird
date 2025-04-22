using Frontend.Services;
using Microsoft.AspNetCore.Components;

namespace Frontend.Components.AuthRedirect;

public partial class AuthRedirect : ComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IAuthService authService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (!await authService.IsAuthorizedAsync)
        {
            NavigationManager.NavigateTo("/Login");
        }
    }
}