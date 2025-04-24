using Frontend.Services;
using Microsoft.AspNetCore.Components;

namespace Frontend.Components.AuthRedirect;

public partial class AuthRedirectBase : ComponentBase
{
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    protected IAuthService AuthService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (!await AuthService.IsAuthorizedAsync)
        {
            NavigationManager.NavigateTo("/Login");
        }
    }
}
