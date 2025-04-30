using Microsoft.AspNetCore.Mvc;

namespace Backend.Extensions;

public static class ControllerExtensions
{
    public static int AuthorizeUser(this ControllerBase controller) 
    {
        if (!controller.HttpContext.Items.ContainsKey("User"))
        {
            throw new UnauthorizedAccessException();
        }
        return (int)controller.HttpContext.Items["User"]!;
    }

    public static bool IsHuman(this ControllerBase controller)
    {
        if (!controller.HttpContext.Items.ContainsKey("IsHuman"))
        {
            return false;
        }

        return controller.HttpContext.Items["IsHuman"] is true;
    }
}