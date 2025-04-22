using Microsoft.AspNetCore.Mvc;

namespace Backend.Core;

public abstract class AuthControllerBase : ControllerBase
{
    protected int? AuthorizeUser()
    {
        if (this.HttpContext.Items.ContainsKey("User"))
        {
            return this.HttpContext.Items["User"] as int?;
        }
        return null;
    }

    protected IActionResult UnauthorizedUser()
    {
        return this.Unauthorized();
    }
}